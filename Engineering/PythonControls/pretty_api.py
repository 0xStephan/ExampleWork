################################################################################
#                    Copyright 2021 V-Space Parkers Pty Ltd
#                       Original Author: Stephan Roberto
#                          stephan@vspaceparkers.com.au
#                             Production Version
#                          Last Modified: 19/08/2021
################################################################################
from wtforms import Form, BooleanField, StringField, PasswordField, validators
from flask import Flask, render_template, redirect, url_for, request, session
from werkzeug.security import generate_password_hash, check_password_hash
from flask import send_from_directory, flash, abort
from pymodbus.client.sync import ModbusTcpClient
from werkzeug.utils import secure_filename
from click._termui_impl import AFTER_BAR
from flask import jsonify, make_response
from flask_sqlalchemy import SQLAlchemy
from configparser import ConfigParser
from flask_qrcode import QRcode
from functools import wraps
from _datetime import date
from pathlib import Path
from opcua import ua
import uuid, jwt, datetime, requests, opcua, random, time, os, xlrd

################################################################################
#   Please note the following application uses the Flask Web Framework
#   Ensure this application is served with a PRODUCTION application server
#   This application has been tested and deemed stable with uWSGI and nginx
################################################################################

app = Flask(__name__)
qrcode = QRcode(app)
UPLOAD_FOLDER = '/home/root/temp_upload'
ALLOWED_EXTENSIONS = {'xlsx'}
app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///api-db.sqlite'
app.config['UPLOAD_FOLDER'] = UPLOAD_FOLDER

################################################################################
#  Load App Configuration
################################################################################

config = ConfigParser()
config.read('config.ini')
app.config['SITE'] = config.get('main', 'SITE')
app.config['SECRET_KEY'] = config.get('main', 'SECRET_KEY')
app.config['SITE_TOPIC'] = config.get('main', 'SITE_TOPIC')
maingate = bool(config.get('main','MAIN_GATE'))

################################################################################
# Database Initialization and Model Declaration
################################################################################
db = SQLAlchemy(app)

# Database Models
class User(db.Model):
    user_id = db.Column(db.String(50), primary_key=True)
    username = db.Column(db.String(50), unique=True)
    name = db.Column(db.String(50))
    password = db.Column(db.String(120))
    spotid = db.Column(db.String(300), default='-1_1')
    email = db.Column(db.String(80), default='none')
    phoneNumber = db.Column(db.String(50), default='none')
    address = db.Column(db.String(100), default='none')
    apartment = db.Column(db.String(100), default='none')
    carrego = db.Column(db.String(50), default='none')
    spotnickname = db.Column(db.String(150), default='none')
    lockout = db.Column(db.Boolean, default=False)
    admin = db.Column(db.Integer)

class SystemDetails(db.Model):
    system_id = db.Column(db.Integer, primary_key=True)
    IP = db.Column(db.String(50))
    port = db.Column(db.String(50))
    OPC = db.Column(db.Boolean, default=True)
    spotregoffset = db.Column(db.String(25), default='none')
    inuseregoffset = db.Column(db.String(25), default='none')
    lastspotregoffset = db.Column(db.String(25), default='none')
    authentication = db.Column(db.Boolean, default=False)
    username = db.Column(db.String(25), default='none')
    password = db.Column(db.String(25), default='none')

class used_qr(db.Model):
    qr_code = db.Column(db.String(250), primary_key=True)

# Form Models
class CreateUserForm(Form):
    username = StringField('Username', [validators.Length(min=3, max=25)])
    name = StringField('Name', [validators.Length(min=3, max=50)])
    spotid = StringField('Spot ID (ie 2_1,3_2)', [validators.Length(min=3, max=300)])
    email = StringField('Email', [validators.Email(message='Invalid email address.')])
    phoneNumber = StringField('Phone Number', [validators.Length(min=3, max=50)])
    address = StringField('Address', [validators.Length(min=3, max=50)])
    apartment = StringField('Apartment', [validators.Length(min=3, max=50)])
    carrego = StringField('Car Rego', [validators.Length(min=1, max=25)])
    password = StringField('Password', [validators.Length(min=5, max=25)])

# Password Generation
def GenerateCustomPassword():
    s1 = "abcdefghijklmnopqrstuvwxyz"
    g1 = "".join(random.sample(s1,4))
    s2 = "1234567890"
    g2 = "".join(random.sample(s2,3))
    return (g1 + g2)

# Allowed File Extension Check
def allowed_file(filename):
    return '.' in filename and \
           filename.rsplit('.', 1)[1].lower() in ALLOWED_EXTENSIONS

# Process Uploaded Excel File
def process_excel(filename):
    result = {}
    workbook = xlrd.open_workbook(app.config['UPLOAD_FOLDER'] + '/' + filename, on_demand=True)
    worksheet = workbook.sheet_by_index(0)

    success_users = []
    failed_users = []

    if (worksheet.nrows < 2):
        result['error'] = 'The uploaded worksheet is empty!'

    # We are working on worksheet without password fields
    elif (worksheet.ncols == 8):
        for row in range(1, worksheet.nrows):
            new_user = CreateUserForm()

            # Username
            new_user.username.data = worksheet.cell_value(row, 0)
            # FullName
            new_user.name.data = worksheet.cell_value(row, 1)
            # Email
            new_user.email.data = worksheet.cell_value(row, 2)
            # Phone  Number
            new_user.phoneNumber.data = worksheet.cell_value(row, 3)
            # Address
            new_user.address.data = worksheet.cell_value(row, 4)
            # Apartment
            new_user.apartment.data = worksheet.cell_value(row, 5)
            # Car Rego
            new_user.carrego.data = worksheet.cell_value(row, 6)
            # SpotID
            new_user.spotid.data = worksheet.cell_value(row, 7)

            generated_password = GenerateCustomPassword()
            new_user.password.data = generated_password
            user = User.query.filter_by(username = new_user.username.data).first()

            # Username already exists
            if user:
                failed_users.append([str(row + 1), new_user.username.data,
                    new_user.name.data, new_user.spotid.data,
                    new_user.apartment.data, 'Username already exists!'])

            # New User Fails Validation
            elif (not new_user.validate()):
                failed_users.append([str(row + 1), new_user.username.data,
                    new_user.name.data, new_user.spotid.data,
                    new_user.apartment.data, new_user.errors])

            else:
                hashed_password = generate_password_hash(new_user.password.data, method='sha256')
                user = User(user_id=str(uuid.uuid4()),
                            username = new_user.username.data,
                            name = new_user.name.data,
                            password = hashed_password,
                            spotid = new_user.spotid.data.replace(" ",""),
                            email = new_user.email.data,
                            phoneNumber = new_user.phoneNumber.data,
                            address = new_user.address.data,
                            apartment = new_user.apartment.data,
                            carrego = new_user.carrego.data,
                            admin=0)

                try:
                    db.session.add(user)
                    db.session.commit()
                    success_users.append([new_user.username.data,
                        generated_password, new_user.name.data,
                        new_user.spotid.data, new_user.apartment.data])

                except Exception as d:
                    print(d)
                    failed_users.append([str(row + 1), new_user.username.data,
                        new_user.name.data, new_user.spotid.data,
                        new_user.apartment.data, str(d)])

    # We are working on worksheet with password fields
    elif (worksheet.ncols == 10):
        for row in range(1, worksheet.nrows):
            new_user = CreateUserForm()

            # Username
            new_user.username.data = worksheet.cell_value(row, 0)
            # FullName
            new_user.name.data = worksheet.cell_value(row, 3)
            # Email
            new_user.email.data = worksheet.cell_value(row, 4)
            # Phone  Number
            new_user.phoneNumber.data = worksheet.cell_value(row, 5)
            # Address
            new_user.address.data = worksheet.cell_value(row, 6)
            # Apartment
            new_user.apartment.data = worksheet.cell_value(row, 7)
            # Car Rego
            new_user.carrego.data = worksheet.cell_value(row, 8)
            # SpotID
            new_user.spotid.data = worksheet.cell_value(row, 9)

            password = worksheet.cell_value(row, 1)
            confirm_password = worksheet.cell_value(row, 2)
            user = User.query.filter_by(username = new_user.username.data).first()

            # Username already exists
            if user:
                failed_users.append([str(row + 1), new_user.username.data,
                    new_user.name.data, new_user.spotid.data,
                    new_user.apartment.data, 'Username already exists!'])
                continue

            # Password and Confirm Password DO NOT match
            elif not (password == confirm_password):
                failed_users.append([str(row + 1), new_user.username.data,
                    new_user.name.data, new_user.spotid.data,
                    new_user.apartment.data, 'Password and Confirm Password DO NOT match'])
                continue

            else:
                new_user.password.data = confirm_password

            if (not new_user.validate()):
                failed_users.append([str(row + 1), new_user.username.data,
                    new_user.name.data, new_user.spotid.data,
                    new_user.apartment.data, new_user.errors])

            else:
                hashed_password = generate_password_hash(new_user.password.data, method='sha256')
                user = User(user_id=str(uuid.uuid4()),
                            username = new_user.username.data,
                            name = new_user.name.data,
                            password = hashed_password,
                            spotid = new_user.spotid.data.replace(" ",""),
                            email = new_user.email.data,
                            phoneNumber = new_user.phoneNumber.data,
                            address = new_user.address.data,
                            apartment = new_user.apartment.data,
                            carrego = new_user.carrego.data,
                            admin=0)

                try:
                    db.session.add(user)
                    db.session.commit()
                    success_users.append([new_user.username.data,
                        new_user.password.data, new_user.name.data,
                        new_user.spotid.data, new_user.apartment.data])

                except Exception as d:
                    print(d)
                    failed_users.append([new_user.username.data, str(d)])

    else:
        result['error'] = 'The uploaded worksheet does not match expected format!'

    result['success_users'] = success_users
    result['failed_users'] = failed_users

    # Ensure file is unlocked prior to deleting
    workbook.release_resources()
    del workbook
    os.remove(app.config['UPLOAD_FOLDER'] + '/' + filename)

    return result

# API Token Verification
def token_required(f):
    @wraps(f)
    def decorated_token(*args, **kwargs):
        token = None

        # Token is stored in request header under key x-access-token
        if 'x-access-token' in request.headers:
            token = request.headers['x-access-token']

        if not token:
            return jsonify({'message' : 'Token is missing!'}), 401

        try:
            # Token is encrypted by last 12 digits of device GUID and SECRET_KEY
            if 'GUID' in request.headers:
                guid = request.headers.get('GUID')
                data = jwt.decode(token, (guid[-12:] + app.config['SECRET_KEY']))

            else:
                data = jwt.decode(token, app.config['SECRET_KEY'])

            current_user = User.query.filter_by(user_id=data['user_id']).first()

        except:
            print("Invalid Token")
            return jsonify({'message' : 'Token is invalid!'}), 401

        return f(current_user, *args, **kwargs)
    return decorated_token

# Admin Panel Login Verification
def login_required(g):
    @wraps(g)
    def decorated_admin(*args, **kwargs):
        if 'token' in session:
            try:
                data = jwt.decode(session['token'], app.config['SECRET_KEY'])
                current_user = User.query.filter_by(user_id=data['user_id']).first()

                if not current_user.admin:
                    return redirect(url_for('loginpage'))

            except:
                return redirect(url_for('loginpage'))

            return g(current_user, *args, **kwargs)

        else:
            return redirect(url_for('loginpage'))

    return decorated_admin

# Root URL
@app.route('/', methods=['GET', 'POST'])
@login_required
def root(current_user):
    return redirect(url_for('home'))

# Login Page
@app.route('/admin/login', methods=['GET', 'POST'])
def loginpage():
    error = None

    # POST request
    if request.method == 'POST':
        user = User.query.filter_by(username=request.form['username'].lower()).first()

        # Valid User
        if user:
            # Correct Password
            if check_password_hash(user.password, request.form['password']):

                if user.admin:
                    # Admin Session will expire after 1 Day
                    session['token'] = jwt.encode({'user_id' : user.user_id,
                        'exp' : datetime.datetime.utcnow() + datetime.timedelta(days=1)},
                        app.config['SECRET_KEY'])

                    flash('You were logged in.')
                    return redirect(url_for('home'))

                else:
                    error = 'User is NOT an admin.'

            # Incorrect Password
            else:
                error = 'Invalid credentials.'

        # Username does not exist
        else:
            error = 'Invalid credentials.'

    return render_template('loginpage.html', error=error)

# Admin Page
@app.route('/admin', methods=['GET', 'POST'])
@login_required
def home(current_user):

    # Second Admin Check
    if (not (current_user.admin)):
        abort(404)

    # POST request
    if request.method == 'POST':
        value = str(request.form['value'])
        field = str(request.form['field']).lower()

        # Must input search term - disallow empty string
        if (len(value) == 0):
            flash("You must input a search term")

        else:
            return redirect(url_for('search', field=field, value=value))

    # Search field options to be rendered on dropdown menu
    options = ['Name', 'SpotID', 'Username', 'PhoneNumber', 'Apartment', 'CarRego', 'Address']

    # Display all existing users on admin page
    #users = User.query.all()
    # Display ONLY non-admin users
    users = User.query.filter_by(admin = 0).all()

    # Show only ADMIN Users
    #users = User.query.filter(User.admin != 0).all()

    #seats = Seat.query.filter(Seat.invite != None).all()

    # Display relevant admin features
    adminlevel = current_user.admin
    print(adminlevel)

    return render_template('index.html', users=users, options=options, site=app.config['SITE'], adminlevel=adminlevel)

# Search for VALUE in FIELD Page
@app.route('/admin/search/<field>/<value>')
@login_required
def search(current_user, field, value):

    # Second Admin Check
    if (not (current_user.admin)):
        abort(404)

    field = field.lower()
    value = value.lower()

    if field == "name":
        users = User.query.filter(User.name.contains(value))

    elif field == "spotid":
        users = User.query.filter(User.spotid.contains(value))

    elif field == "username":
        users = User.query.filter(User.username.contains(value))

    elif field == "apartment":
        users = User.query.filter(User.apartment.contains(value))

    elif field == "carrego":
        users = User.query.filter(User.carrego.contains(value))

    elif field == "address":
        users = User.query.filter(User.address.contains(value))

    elif field == "phonenumber":
        users = User.query.filter(User.phoneNumber.contains(value))

    else:
        flash("Error with field: " + str(field))
        users = User.query.filter(User.name.contains("_a_#bccbaabccbaabccbaabccba"))
        value = None
        field = None

    if (users.count() == 0):
        flash('No users were found')

    else:
        flash(str(users.count()) + ' Users found')

    return render_template('search.html', users=users, field=field, value=value)

# Create New Admin User Page
@app.route('/admin/createadmin', methods=['GET','POST'])
@login_required
def createadmin(current_user):

    # Form has been submitted - POST request
    if request.method == 'POST':
        result = request.form

        try:
            for key, value in result.items():
                if (key == 'username'):
                    if(value == ''):
                        raise ValueError('Username field CANNOT be empty')

        except Exception as e:
            flash(e)
            print(e)

    return render_template('createadmin.html')

# Create New User Page
@app.route('/admin/createuser', methods=['GET', 'POST'])
@login_required
def createuser(current_user):
    # Second Admin Check
    if (not (current_user.admin)):
        abort(404)

    form = CreateUserForm(request.form)

    # Form has been submitted - POST request
    if request.method == 'POST':
        if form.validate():
            user = User.query.filter_by(username = form.username.data).first()

            # Username already exists
            if user:
                flash('Username already exists!')

            else:
                hashed_password = generate_password_hash(form.password.data, method='sha256')
                new_user = User(user_id=str(uuid.uuid4()),
                            username = form.username.data,
                            name = form.name.data,
                            password = hashed_password,
                            spotid = form.spotid.data.replace(" ",""),
                            email = form.email.data,
                            phoneNumber = form.phoneNumber.data,
                            address = form.address.data,
                            apartment = form.apartment.data,
                            carrego = form.carrego.data,
                            admin=False)

                try:
                    db.session.add(new_user)
                    db.session.commit()
                    flash('User successfully created - ' + form.username.data +
                        ' : ' + form.password.data)

                except Exception as e:
                    print(e)
                    flash(e)

    else:
       form.password.data = GenerateCustomPassword()

    return render_template('createuser.html', form=form)

# Create Multiple Users
@app.route('/admin/createmultiple', methods=['GET', 'POST'])
@login_required
def processmultipleusers(current_user):

    # Second Admin Check
    if (not (current_user.admin)):
        abort(404)

    # POST request
    if request.method == 'POST':
        # Check if the POST request has the file attached
        if 'file' not in request.files:
            flash('No file uploaded')
            return redirect(request.url)

        file = request.files['file']
        # If user does not select file, browser can also
        # submit an empty attachment without filename
        if file.filename == '':
            flash('No selected file')
            return redirect(request.url)

        # Check if file is of allowed extension (xlsx)
        if file:
            if allowed_file(file.filename):

                filename = secure_filename(file.filename)
                file.save(os.path.join(app.config['UPLOAD_FOLDER'], filename))
                result = process_excel(filename)

                # Case where we encounter errors in file processing
                if ((len(result) == 1) or (len(result) == 3)):
                    flash(result['error'])
                    success_users = []
                    failed_users = []

                else:
                    flash('Successfully proccessed uploaded file')
                    success_users = result['success_users']
                    failed_users = result['failed_users']

                return render_template('createmultipleuser.html', success_users=success_users, failed_users=failed_users)

            else:
                flash('Invalid file format uploaded')

    return render_template('createmultiplehome.html')

# System Details Page
@app.route('/admin/editsystem')
@login_required
def homesystem(current_user):

    # Only Super Admin and Admin Can Access This Page
    if (not ((current_user.admin == 1) | (current_user.admin == 2))):
        abort(404)

    systems = SystemDetails.query.all()

    return render_template('indexsystem.html', systems=systems)

# Admin Accounts Page
@app.route('/admin/editadmin')
@login_required
def homeadminacc(current_user):

    # Only Super Admin and Admin Can Access This Page
    if (not ((current_user.admin == 1) | (current_user.admin == 2))):
        abort(404)

    # If Super Admin logged in, Show ALL Admin Accounts
    if (current_user.admin == 1):
        users = User.query.filter(User.admin != 0).all()

    else:
        users = User.query.filter((User.admin == 2) | (User.admin == 3)).all()

    adminlevel = current_user.admin

    return render_template('indexadminacc.html', users=users, adminlevel=adminlevel)

# Enable / Disable Main Gate
@app.route('/admin/editsystem/maingate', methods=['GET','POST'])
@login_required
def enablemaingate(current_user):
    # Only Super Admin and Admin Can Access This Page
    if (not ((current_user.admin == 1) | (current_user.admin == 2))):
        abort(404)

    # Access maingate global variable and trigger it
    global maingate

    if (maingate):
        # We want to disable MAIN_GATE
        config.set('main', 'MAIN_GATE', '')
        print('Main Gate is ENABLED')

    else:
        # We want to enable MAIN_GATE
        config.set('main', 'MAIN_GATE','True')
        print('Main Gate is DISABLED')

    # Write new changes to config file
    with open('config.ini', 'w') as f:
        config.write(f)

    # Load new config value for maingate
    maingate = bool(config.get('main','MAIN_GATE'))

    if (maingate):
        flash('Main Gate is now Enabled')
        return redirect(url_for('homesystem'))

    else:
        flash('Main Gate is now Disabled')
        return redirect(url_for('homesystem'))

    return redirect(url_for('homesystem'))

# Edit Individual System Details Page
@app.route('/admin/editsystem/<system_id>', methods=['GET', 'POST'])
@login_required
def editsystem(current_user, system_id):

    # Only Super Admin and Admin Can Access This Page
    if (not ((current_user.admin == 1) | (current_user.admin == 2))):
        abort(404)

    system = SystemDetails.query.filter_by(system_id=system_id).first()

    if not system:
            flash('Invalid System ID')
            return redirect(url_for('home'))

    current = str(system.OPC)
    current2 = str(system.authentication)

    options = ['True', 'False']

    # If form has been submitted update system details
    if request.method == 'POST':
        result = request.form

        for key, value in result.items():

            if key == 'IP':
                system.IP = value

            elif key == 'port':
                system.port = value

            elif key == 'OPC':
                if value == 'True':
                    system.OPC = True
                else:
                    system.OPC = False
                current = str(value)

            elif key == 'spotregoffset':
                system.spotregoffset = value

            elif key == 'inuseregoffset':
                system.inuseregoffset = value

            elif key == 'lastspotregoffset':
                system.lastspotregoffset = value

            elif key == 'authentication':
                if value == 'True':
                    system.authentication = True
                else:
                    system.authentication = False
                current2 = str(value)

            elif key == 'username':
                system.username = value

            elif key == 'password':
                system.password = value

            db.session.commit()
            system = SystemDetails.query.filter_by(system_id=system_id).first()

        flash('System ' + str(system.system_id) + ' successfully updated')

    return render_template('editsystem.html', system=system, current=current,
        current2=current2, options=options)

# Edit Site Key Page
@app.route('/admin/editkey', methods=['GET', 'POST'])
@login_required
def editkey(current_user):

    # Only Super Admin and Admin Can Access This Page
    if (not ((current_user.admin == 1) | (current_user.admin == 2))):
        abort(404)

    # If form has been submitted update site key
    if request.method == 'POST':
        result = request.form

        # Empty string
        if ((not request.form['sitekey']) | (not request.form['sitename'])):
            flash('Site Name / Key CANNOT be empty')

        # We have a string so update config
        else:
            config.set('main', 'SITE', str(request.form['sitename']))
            config.set('main', 'SECRET_KEY', str(request.form['sitekey']))
            # Update config.ini file
            with open('config.ini', 'w') as f:
                config.write(f)

            # Update app config with config file
            app.config['SITE'] = config.get('main', 'SITE')
            app.config['SECRET_KEY'] = config.get('main', 'SECRET_KEY')
            flash('Site Values have been successfully updated')
            Path('/home/root/pretty_api.ini').touch()

    site_name = app.config['SITE']
    site_key = app.config['SECRET_KEY']


    return render_template('editsitekey.html', site_name=site_name, site_key=site_key, site=app.config['SITE'])

# Edit Admin User Details Page
@app.route('/admin/editadmin/<user_id>', methods=['GET', 'POST'])
@login_required
def editadmin(current_user, user_id):

    # Only Super Admin and Admin Can Access This Page
    if (not ((current_user.admin == 1) | (current_user.admin == 2))):
        abort(404)

    user = User.query.filter_by(user_id=user_id).first()

    if not user:
            flash('Invalid user ID')
            return redirect(url_for('homeadminacc'))

    if (current_user.admin == 1):
        options = ['Super Admin', 'Admin', 'Site Manager']

    else:
        options = ['Admin', 'Site Manager']

    if request.method == 'POST':
        result = request.form

        if request.form['submit_button'] == 'Delete User':
            if (current_user.user_id == user_id):
                flash('CANNOT Delete Currently Logged In User')

            else:
                deleted_username = user.username
                #db.session.delete(user)
                #db.session.commit()
                flash(deleted_username + ' successfully deleted')
                return redirect(url_for('homeadminacc'))

        elif request.form['submit_button'] == 'Reset Password':
            if (current_user.user_id == user_id):
                flash('CANNOT Reset Currently Logged In User')

            else:
                generated_password = GenerateCustomPassword()

                hashed_password = generate_password_hash(generated_password, method='sha256')
                user.password = hashed_password

                try:
                    #db.session.commit()
                    flash('New Credentials - ' + user.username + ' : ' + generated_password)

                except Exception as e:
                    print(e)
                    flash(e)

        elif request.form['submit_button'] == 'Update Values':
            try:
                for key,value in result.items():
                    if (key == 'username'):
                        check_user = User.query.filter_by(user_id=user_id).first()

                        # We are changing the username
                        if (not (check_user.username == value)):
                            existing_username = User.query.filter_by(username=value).first()

                            # Requested Username already exists
                            if (existing_username):
                                raise ValueError('Username ALREADY exists. Please use a different username.')

                        user.username = value

                    elif (key == 'admintype'):
                        if ((user.admin == 1) & (value != 'Super Admin')):
                            flash('CANNOT Downgrade a Super Admin')

                        elif (value == 'Super Admin'):
                            user.admin = 1
                        elif (value == 'Admin'):
                            user.admin = 2
                        else:
                            user.admin = 3

                    elif (key == 'spotid'):
                        user.spotid = value

                    db.session.commit()
                flash('Successfully updated user values')

            except Exception as e:
                print(e)
                flash(e)

    # Preselect User Admin Type
    if (user.admin == 1):
        current='Super Admin'
    elif (user.admin == 2):
        current='Admin'
    elif (user.admin == 3):
        current='Site Manager'

    return render_template('editadminacc.html', user=user, options=options, current=current)

# Edit User Details Page
@app.route('/admin/edituser/<user_id>', methods=['GET', 'POST'])
@login_required
def edituser(current_user, user_id):
    user = User.query.filter_by(user_id=user_id).first()

    if not user:
            flash('Invalid user ID')
            return redirect(url_for('home'))

    # If form has been submitted update user values
    if request.method == 'POST':
        result = request.form

        if request.form['submit_button'] == 'Trigger Lockout':
            user.lockout = not user.lockout
            db.session.commit()
            flash('User Lockout changed to ' + str(user.lockout))

        elif request.form['submit_button'] == 'Delete User':
            deleted_username = user.username
            db.session.delete(user)
            db.session.commit()
            flash(deleted_username + ' successfully deleted')
            return redirect(url_for('home'))


        elif request.form['submit_button'] == 'Reset Password':
            if user.admin:
                flash('Cannot reset administrative accounts')
                return render_template('edituser.html', user=user)

            #generated_password = GenerateCustomPassword()
            # Password will reset to vspace + spotID
            # ie: vspace24
            generated_password = str('vspace') + str(user.spotid.split('_')[0])

            hashed_password = generate_password_hash(generated_password, method='sha256')
            user.password = hashed_password

            try:
                db.session.commit()
                flash('New Credentials - ' + user.username + ' : ' + generated_password)

            except Exception as e:
                print(e)
                flash(e)

        else:
            try:
                for key, value in result.items():
                    if key == 'username':
                        check_user = User.query.filter_by(user_id=user_id).first()

                        # We are changing the username
                        if (not (check_user.username == value)):
                            existing_username = User.query.filter_by(username=value).first()

                            # Requested Username already exists
                            if (existing_username):
                                raise ValueError('Username ALREADY exists. Please use a different username.')

                        user.username = value.lower()

                    elif key == 'name':
                        user.name = value

                    elif key == 'spotid':
                        user.spotid = value

                    elif key == 'email':
                        user.email = value

                    elif key == 'phoneNumber':
                        user.phoneNumber = value

                    elif key == 'address':
                        user.address = value

                    elif key == 'apartment':
                        user.apartment = value

                    elif key == 'carrego':
                        user.carrego = value

                db.session.commit()
                flash('Successfully updated user values')

            except Exception as e:
                print(e)
                flash(e)

        user = User.query.filter_by(user_id=user_id).first()

    else:
        user = User.query.filter_by(user_id=user_id).first()

    return render_template('edituser.html', user=user)

# View Logs Page
@app.route('/admin/viewlogs')
@login_required
def viewlogs(current_user):
    Path('/home/root/logs/' + datetime.datetime.now().strftime("%b-%Y") + '-tokenlog.txt').touch()
    f1 = open('logs/' + datetime.datetime.now().strftime("%b-%Y") + '-tokenlog.txt', 'r')
    tokenlog = f1.read()

    Path('/home/root/logs/' + datetime.datetime.now().strftime("%b-%Y") + '-callspotlog.txt').touch()
    f2 = open('logs/' + datetime.datetime.now().strftime("%b-%Y") + '-callspotlog.txt', 'r')
    callspotlog = f2.read()

    f1.close()
    f2.close()

    return render_template('viewlogs.html', tokenlog=tokenlog, callspotlog=callspotlog)

# QR Code Generation Page
@app.route('/admin/qrcode', methods=['GET', 'POST'])
@login_required
def qrmain(current_user):

    # If form has been submitted generate QR code
    if request.method == 'POST':
        result = request.form

        for key, value in result.items():
            if key == 'spotid':
                if value == '':
                    flash('Spot ID term must not be empty')
                    spot_id = ''
                    return render_template('qrmain.html', spot_id = spot_id)

                else:
                    try:
                        spot_id = int(value)
                    except:
                        spot_id = ''
                        flash('Spot ID is not a valid integer')
                        return render_template('qrmain.html', spot_id = spot_id)

            if key == 'systemid':
                if (value == ''):
                    flash('System ID term must not be empty')
                    system_id = ''
                else:
                    try:
                        system_id = int(value)

                    except Exception as e:
                        print(e)
                        system_id = ''
                        flash('Input is not a valid integer')


        if (system_id != '') & (spot_id != ''):
            spot_string = str(spot_id) + '_' + str(system_id)
            qr_string = jwt.encode({'spotid' : str(spot_string),
                'exp' : datetime.datetime.utcnow() + datetime.timedelta(days=14)},
                app.config['SECRET_KEY']).decode('utf-8')

            return render_template('qrcode.html', generated_string = qr_string,
                spot_id = spot_id, system_id = system_id)

    else:
        spot_id = ''
        system_id = ''

    return render_template('qrmain.html', spot_id=spot_id)

# Logged Out Page
@app.route('/admin/endsession')
def welcome():
    return render_template('welcome.html')

# Logout Method - Remove Session Variable
@app.route('/admin/logout')
@login_required
def logout(current_user):
    session.pop('token', None)

    return redirect(url_for('welcome'))

################################################################################
#                            END OF ADMIN PANEL                                #
################################################################################

################################################################################
#                               MOBILE APP API                                 #
################################################################################

# API Register User
@app.route('/api/register/<query_string>', methods=['GET', 'POST'])
def register_user(query_string):
    # Generation of Query String
    # jwt.encode({'spotid' : '1_1', 'exp' : datetime.datetime.utcnow() + datetime.timedelta(days=14)}, SECRET_KEY).decode('utf-8')

    # Step 1 Verify Valid Query String
    if request.method == 'GET':
        try:
            data = jwt.decode(query_string, app.config['SECRET_KEY'])
            spotid = data['spotid']

            # Check to see if qr is not used
            qr = used_qr.query.filter_by(qr_code = str(query_string)).first()
            if qr:
                return make_response('This QR code has already been used', 409)

        except Exception as e:
            print(e)
            print("QR Validation Error:")
            return make_response('Could not verify', 401)

            # If we got here we have a valid query string
            # Proceed to return SpotID to be pre-filled
            # entry in App Registration Page

        return jsonify({"SpotID" : str(spotid), "Message" : "Message"})

    # Step 2 User Submit details
    else:
        # Check if submitted spotID matches spotID from query string
        data = request.get_json()
        query_data = jwt.decode(query_string, app.config['SECRET_KEY'])

        if not (data['spotid'] == query_data['spotid']):
            return make_response('Could not verify', 401)

        # Check to see if qr is not used
        qr = used_qr.query.filter_by(qr_code = query_string).first()
        if qr:
            return make_response('This QR code has already been used', 409)

        # Check to see if username is unique
        user = User.query.filter_by(username = data['username'].lower()).first()

        if user:
            return make_response('Username already exists', 409)

        hashed_password = generate_password_hash(data['password'], method='sha256')

        new_user = User(user_id=str(uuid.uuid4()),
                    username = data['username'].lower(),
                    name = data['name'],
                    password = hashed_password,
                    spotid = data['spotid'],
                    email = data['email'],
                    phoneNumber = data['phonenumber'],
                    address = data['address'],
                    apartment = data['apartment'],
                    carrego = data['carrego'],
                    admin=False)

        new_qr = used_qr(qr_code = str(query_string))

        try:
            db.session.add(new_user)
            db.session.add(new_qr)
            db.session.commit()
            return make_response('Successfully created user', 200)

        except Exception as e:
            print(e)
            print("QR Error Creating User:")
            return make_response('An error occured', 400)

# API Online Check
@app.route('/online')
def online():

    return make_response('We are online!', 200)


# API Login
@app.route('/login')
def login():
    auth = request.authorization

    if not auth or not auth.username or not auth.password:
        return make_response('Could not verify', 401,
            {'WWW-Authenticate' : 'Basic realm="Login required!"'})

    user = User.query.filter_by(username=auth.username).first()

    if not user:
        # Try logging in with email instead
        if ('@' in auth.username):
            user = User.query.filter_by(email=auth.username).first()

        else:
            return make_response('Could not verify', 401,
                {'WWW-Authenticate' : 'Basic realm="Login required!"'})

    if not user:
        return make_response('Could not verify', 401,
            {'WWW-Authenticate' : 'Basic realm="Login required!"'})

    if check_password_hash(user.password, auth.password):
        # Token is encrypted by the device
        # last 12 digits of GUID and SECRET_KEY
        try:
            guid = request.headers.get('GUID')
        except:
            return make_response('Could not verify', 401,
                {'WWW-Authenticate' : 'Basic realm="GUID required!"'})

        # Token expirary in 10 years
        token = jwt.encode({'user_id' : user.user_id,
            'exp' : datetime.datetime.utcnow() + datetime.timedelta(days=3650)},
            (guid[-12:] + app.config['SECRET_KEY']))

        if user.lockout:
            return make_response('User Locked Out', 403,
                {'WWW-Authenticate' : 'Basic realm="Banned!"'})

        # We have a valid token
        f = open('logs/' + datetime.datetime.now().strftime("%b-%Y") + '-tokenlog.txt', 'a+')
        f.write('\n############################\n')
        f.write(datetime.datetime.now().strftime("%d %b %Y %H:%M:%S"))
        f.write('\n')
        try:
            f.write(request.headers.get('X-Forwarded-For') + '\n')
        except:
            pass
        try:
            f.write(request.headers.get('User-Agent') + '\n')
        except:
            pass
        try:
            f.write('App Version: ' + request.headers.get('App-Version') + '\n')
        except:
            pass
        try:
            f.write('GUID: '+ request.headers.get('GUID') + '\n')
        except:
            pass
        f.write('Name: ' + user.name + '\n')
        f.write('Username: ' + user.username + '\n')
        f.close()

        return jsonify({'token' : token.decode('UTF-8')})

    return make_response('Could not verify', 401,
        {'WWW-Authenticate' : 'Basic realm="Login required!"'})

# API Change Password
@app.route('/api/Account/ChangePassword', methods=['POST'])
@token_required
def change_password(current_user):
    data = request.get_json()

    # Old password supplied correctly
    if check_password_hash(current_user.password, data['old_password']):
        try:
            hashed_password = generate_password_hash(data['new_password'], method='sha256')

        except:
            return make_response('Error changing password', 500,
                {'WWW-Authenticate' : 'Basic realm="Bad password!"'})

        current_user.password = hashed_password
        db.session.commit()
        return make_response('Successfully changed', 200)

    else:
      return make_response('Could not verify', 401,
        {'WWW-Authenticate' : 'Basic realm="Login required!"'})

# API Change Spot Nicknames
@app.route('/api/Account/ChangeSpotNickName', methods=['POST'])
@token_required
def change_spot_nickname(current_user):
    data = request.get_json()

    try:
        new_nicknames = data['new_nicknames']

    except:
        return make_response('Error reading nicknames', 500)

    current_user.spotnickname = new_nicknames
    db.session.commit()

    return make_response('Successfully changed', 200)

# API Get User Values
@app.route('/api/values', methods=['GET'])
@token_required
def get_values(current_user):

    print('User API Values')
    print(str(maingate))

    if (str(maingate) == 'True'):
        maingateval = 'true'
    else:
        maingateval = 'false'

    return jsonify({"SpotID" : current_user.spotid,
                    "Name" : current_user.name,
                    "Username" : current_user.username,
                    "SpotNickName" : current_user.spotnickname,
                    "MainGate" : maingateval,
                    "Support" : True,
                    "Site" : str(app.config['SITE']),
                    "Topic" : str(app.config['SITE_TOPIC']),
                    "Apartment" : current_user.apartment})

# API Call Spot
@app.route('/api/values/<request_id>/<cycle_type>', methods=['GET'])
@token_required
def request_gate(current_user, request_id, cycle_type):
    print("Received Request To Call Spot:")
    print(request_id)

    # User account is locked out
    if current_user.lockout:
        return jsonify({"Status" : "403",
                    "Message" : "User Locked Out"})

    spots = current_user.spotid.split(',')
    spotfound = False

    if (request_id == "0_0"):
        return jsonify({"Status" : "200",
                        "Message" : "Processed"})

    # Check if user has permission to requested spot
    for spot in spots:
        if request_id == spot:
            spotfound = True

    # User does not have permission to requested spot
    if (not spotfound):
        return jsonify({"Status" : "401",
                    "Message" : "Invalid Spot"})

    print("Call Spot BEFORE LOG")

    spot_id = request_id.split('_')[0]
    system_id = request_id.split('_')[1]

    system = SystemDetails.query.filter_by(system_id=system_id).first()

    # Communicate to relevant system (OPC or Modbus)
    if system.OPC:
        output = opc_comms(spot_id, system_id, cycle_type)
    else:
        output = modbus_comms(spot_id, system_id, cycle_type)

    print("Call Spot AFTER Comms Function")
    #print(output)
    # Call Gate
    # output = call_gate(system_id, gate_id)
    # Status 220 is Custom Message
    # 200 is success
    # 220 is custom
    # OK => "200";
    # Unauth => "401";
    # Banned => "403";
    # Offline => "503";
    # Error => "500";
    # BadPassword => "204";
    # Accepted => "202";
    # Custom => "220";

    # If we reach here the user has permission to call spot
    f = open('logs/' + datetime.datetime.now().strftime("%b-%Y") + '-callspotlog.txt', 'a+')
    f.write('\n############################\n')
    f.write(datetime.datetime.now().strftime("%d %b %Y %H:%M:%S"))
    f.write('\n')
    try:
        f.write(request.headers.get('X-Forwarded-For') + '\n')
    except:
        pass
    try:
        f.write(request.headers['User-Agent'] + '\n')
    except:
        pass
    try:
        f.write('App Version: '+ request.headers.get('App-Version') + '\n')
    except:
        pass
    try:
        f.write('GUID: '+ request.headers.get('GUID') + '\n')
    except:
        pass
    f.write('Name: ' + current_user.name + '\n')
    f.write('Username: ' + current_user.username + '\n')
    f.write('Called Spot: ' + request_id + '\n')
    f.write('Output: ' + str(output) + '\n')
    #f.write('############################' + '\n')
    f.close()

    print(output)

    if (output == "301"):
        return jsonify({"Status" : "301",
                        "MessageTitle" : "Previous Session Detected",
                        "Message" : "The system has detected that a previous session is still open. Please close the gate to end the session.",
                        "Support" : False})

    elif (output == "AlreadyClosing"):
        return jsonify({"Status" : "220",
                        "MessageTitle" : "Already Closing",
                        "Message" : "The gate is already closing.",
                        "Support" : False})

    elif (output == "GateClose200"):
        return jsonify({"Status" : "220",
                        "MessageTitle" : "Whoops",
                        "Message" : "The system cannot detect an open gate. Please try again.",
                        "Support" : False})

    else:
        return jsonify({"Status" : str(output),
                    "MessageTitle" : "Alert",
                    "Message" : "Processed",
                    "Support" : False})

def modbus_comms(spot_id, system_id, cycle_type):
    system = SystemDetails.query.filter_by(system_id=system_id).first()

    # Cycle_type Lookup Table
    # 1 => Start Cycle (Calling a Spot)
    # 0 => End Cycle (Closing Gate)


    # PLC Status Register Value Lookup Table
    #
    # 200 => Ready to be used
    # 503 => BUSY / IN operation
    # 101 => GATE is OPEN
    # 102 => Machine Error
    # 110 => Gate is CLOSING
    # 103 => Machine in MANUAL MODE (Cannot Use App)
    #
    # 3 registers to look out for
    # R0 (Call Spot Register)
    # R1 (Status Code)
    # R3 (Last User Called)

    try:
        client = ModbusTcpClient(system.IP, int(system.port))

        # Check if stacker is in use
        inuse = client.read_holding_registers(int(system.inuseregoffset), 1)
        print("In Use Value:")
        print(inuse.registers[0])

        # Check if stacker is not locked by another user
        lockedspot = client.read_holding_registers(int(system.lastspotregoffset), 1)
        print("Locked Spot:")
        print(lockedspot.registers[0])

        # Emergency Button is Pressed
        if (inuse.registers[0] == 501):
            client.close()
            return "501"

        # Case where stacker reports in use
        elif (inuse.registers[0] == 205):

            # Cycle needs to END but user is requesting to START
            if ((int(cycle_type) == 1) & (int(lockedspot.registers[0]) == int(spot_id))):
                client.close()
                return "301"

            # Stacker is still in use
            client.close()
            return "202"

        # Gate is already closing
        elif (inuse.registers[0] == 110):

            # Detect if user is trying to close the gate  or STARTING
            #       a new session
            client.close()

            # User is trying to call their spot but gate is closing
            if (int(cycle_type) == 1):
                return "202"

            else:
                return "AlreadyClosing"


        # Gate is OPEN
        if (inuse.registers[0] == 101):

            # Cycle needs to END but user is requesting to START
            if ((int(cycle_type) == 1) & (int(lockedspot.registers[0]) == int(spot_id))):
                client.close()
                return "301"

            # Gate is OPEN and CORRECT user is requesting to close it
            elif (int(lockedspot.registers[0]) == int(spot_id)):
                client.write_register(int(system.spotregoffset), int(spot_id))
                client.close()
                return "200"

            else:
                client.close()
                return "202"

        # Close Gate and Sensors are Interrupting
        #       Front Sensor is Interrupted
        if (int(inuse.registers[0]) == int(505)):
            print("Inside 505 IF Statement")
            print("Locked Spot:")
            print(lockedspot.registers[0])
            print("Spot_ID:")
            print(spot_id)

            # Cycle needs to END but user is requesting to START
            if ((int(cycle_type) == 1) & (int(lockedspot.registers[0]) == int(spot_id))):
                client.close()
                return "301"


            # User is requesting to Close GATE
            elif (int(lockedspot.registers[0]) == int(spot_id)):
                client.close()
                print("Front Sensor Error")
                return "505"

            else:
                # Stacker is still in use by someone else
                client.close()
                return "202"

        #       Rear Sensor is Interrupted
        if (int(inuse.registers[0]) == int(506)):

            # Cycle needs to END but user is requesting to START
            if ((int(cycle_type) == 1) & (int(lockedspot.registers[0]) == int(spot_id))):
                client.close()
                return "301"

            # User is requesting to Close GATE
            elif (int(lockedspot.registers[0]) == int(spot_id)):
                client.close()
                print("Rear Sensor Error")
                return "506"

            else:
                # Stacker is still in use by someone else
                client.close()
                return "202"


        # Requesting to close gate but stacker reading 200
        if ((inuse.registers[0] == 200) & (int(cycle_type) == 0)):
            client.close()
            return "GateClose200"

        # Stacker is free to be used
        # Let's call the spot
        elif ((inuse.registers[0] == 200)):
            client.write_register(int(system.spotregoffset), int(spot_id))
            client.close()
            return "200"


        # Some other kind of error
        else:
            client.close()
            print("500 Error ELSE statement")
            print("Cycle Type:")
            print(cycle_type)
            return "500"


    # Error during communication
    except Exception as e:

            print("Communication Error:")
            print(e)
            try:
                client.close()
            except:
                pass

            return "500"

def opc_comms(spot_id, system_id, cycle_type):
    try:
        system = SystemDetails.query.filter_by(system_id=system_id).first()

        client = opcua.Client("opc.tcp://" + str(system.IP) + ":" + str(system.port))
        #client = opcua.Client("opc.tcp://192.168.0.135:62652")


        if system.authentication:
            client.set_user((system.username))
            client.set_password((system.password))

        client.connect()

        # Get nodes
        inusenode = client.get_node(str(system.inuseregoffset))
        lastspotnode = client.get_node(str(system.lastspotregoffset))
        spotnode = client.get_node(str(system.spotregoffset))

        # Cycle needs to END but user is requesting to START
        if ((int(cycle_type) == 1) & (lastspotnode.get_value() == int(spot_id))):
            client.disconnect()
            return "301"


        # Case where stacker reports in use
        if (inusenode.get_value() == 1):

            # Try read again after a few seconds
            time.sleep(3 + random.randint(1,5))

            # If stacker still in use return busy code
            if (inusenode.get_value() == 1):

                # Stacker is still in use
                client.disconnect()
                return "202"

        # Check if stacker is not locked by another user
        # Gate open
        if not (lastspotnode.get_value() == 0):

            # Gate is open and correct user is requesting to close it
            if (int(lastspotnode.get_value()) == int(spot_id)):
                ## spotnode.set_value(0, spotnode.get_data_type_as_variant_type())
                spotnode.set_value(ua.DataValue(ua.Variant(int(spot_id), spotnode.get_data_type_as_variant_type())))
                client.disconnect()
                return "200"

            else:
                client.disconnect()
                return "202"



        # Stacker is free to be used
        ## spotnode.set_value(int(spot_id), spotnode.get_data_type_as_variant_type())
        ## inusenode.set_value(1, inusenode.get_data_type_as_variant_type())

        spotnode.set_value(ua.DataValue(ua.Variant(int(spot_id), spotnode.get_data_type_as_variant_type())))
        #inusenode.set_value(ua.DataValue(ua.Variant(1, inusenode.get_data_type_as_variant_type())))

        # Wait 3 seconds to ensure communication
        time.sleep(3)
        ## inusenode.set_value(0, inusenode.get_data_type_as_variant_type())
        #inusenode.set_value(ua.DataValue(ua.Variant(0, inusenode.get_data_type_as_variant_type())))
        client.disconnect()

        return "200"

    except Exception as e:
            # Error during communication
            print("Communication Error:")
            print(e)
            try:
                client.disconnect()
            except:
                pass

            return "500"
# , ssl_context='adhoc' app.run

if __name__ == '__main__':
    app.run(host="0.0.0.0", debug=True)
    app.add_url_rule('/favicon.ico',
                 redirect_to=url_for('static', filename='favicon.ico',
                 mimetype='image/vnd.microsoft.icon'))
    # , ssl_context='adhoc'

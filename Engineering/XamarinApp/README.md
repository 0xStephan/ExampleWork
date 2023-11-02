# User Interface App for Residents

This application allows users to call their vehicle from the car stacker / car elevator.

The typical user journey is as follows:
1) User open app and clicks "Connect"
   i) The app connects to a private network that has access to the IoT device (this is the middle man between the user and the engineering controls network)

2) The profile of the user loads and the user is able to call their relevant spot/s (users can have multiple vehicles and multiple parking spaces)

3) The app communicates to the IoT via the API service running on the IoT

4) IoT translates request and communicates to the Engineering Controls systems

5) App communicates to the resident on the status of their request (successful, busy, not online etc)

6) User retrieves their vehicle from the machine

7) User then notifies the app that they are complete with the process and to end the request cycle (for safety reasons the system cannot be used until a user as closed their previous request)

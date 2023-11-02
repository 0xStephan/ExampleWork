// SPDX-License-Identifier: MIT
pragma solidity ^0.8.4;

import "@openzeppelin/contracts/access/Ownable.sol";
import "hardhat/console.sol";

contract DoubleDoge is Ownable {
    uint256 private _startPrice = 0.01 ether;
    uint256 private _turn = 1;
    uint256 private _round = 1;
    uint256 private _deposits = 0;
    mapping (address => uint256) private _depositedFirst;
    mapping (address => uint256) private _depositedSecond;
    address[] private _keysFirst;
    address[] private _keysSecond;

    address private _taxAddress;
    uint256 private _tax;

    uint256 private _power = 0;
    uint256 private _multiply = 0;

    uint256 private _payout = _startPrice;
    uint256 private _times = 0;

    constructor() {
        _taxAddress = msg.sender;
    }

    event deposit_event(uint _value, uint _remaining);
    event payout_event(address indexed _from, uint _value);

    function deposit(uint256 qty) public payable {
        require(msg.value >= _startPrice, "y u so poor lol");
        require((msg.value) >= (qty * _startPrice), "incorrect value");
        require(((_turn*2)-_deposits) >= qty, "QTY too large");


        if (_round % 2 != 0) {
            // If user has already deposited
            if (_depositedFirst[msg.sender] >= 1) {
                _depositedFirst[msg.sender] += qty;
            } 
            // User has not deposited yet
            else {
                _keysFirst.push(msg.sender);
                _depositedFirst[msg.sender] = qty;
            }
        } else {
            // If user has already deposited
            if (_depositedSecond[msg.sender] >= 1) {
                _depositedSecond[msg.sender] += qty;
            } 
            // User has not deposited yet
            else {
                _keysSecond.push(msg.sender);
                _depositedSecond[msg.sender] = qty;
            }
        }

        _deposits += qty;

        emit deposit_event(qty, (_turn*2)-qty);

        // We are ready to start paying rewards
        if (_deposits == (_turn * 2)) {
            _deposits=0;
            if (_round > 1) {
                rewards();
            }
            _turn = _turn * 2;
            _round++;
        }
    }

    function rewards() internal {

        // Iterate through loop of addresses for previous _round
        // and pay them out
        if (_round % 2 == 0) {
            // We pay from First arrays
            for (uint256 i=0; i< (_keysFirst.length); i++) {
                _times = _depositedFirst[_keysFirst[i]];
                _payout = ((_startPrice/100) * 95 * _times * 2);
                _depositedFirst[_keysFirst[i]] = 0;
                _keysFirst[i].call{value: _payout}("");
                emit payout_event(_keysFirst[i], _payout);
                _payout = 0;
            }
            delete _keysFirst;

            // Pay tax to devs and marketing
            _power = _round - 1;
            _multiply = 2 ** _power;
            _tax = (_startPrice/100) * _multiply * 8;
            _taxAddress.call{value: _tax}("");
            _tax = 0;
        }
        else {
            // We pay from Second arrays
            for (uint256 i=0; i< (_keysSecond.length); i++) {
                _times = _depositedSecond[_keysSecond[i]];
                _payout = ((_startPrice/100) * 95 * _times * 2);
                _depositedSecond[_keysSecond[i]] = 0;
                _keysSecond[i].call{value: _payout}("");
                emit payout_event(_keysSecond[i], _payout);
                _payout = 0;
            }
            delete _keysSecond;

            // Pay tax to devs and marketing
            _power = _round - 1;
            _multiply = 2 ** _power;
            _tax = (_startPrice/100) * _multiply * 8;
            _taxAddress.call{value: _tax}("");
            _tax = 0;
        }
    }

    function getBalance() view public returns (uint256) {
        return address(this).balance;
    }

    function getRound() view public returns (uint256) {
        return _round;
    }

    function getDeposits() view public returns (uint256) {
        return _deposits;
    }

    function withdraw(address to, uint256 amount) public payable onlyOwner {
        to.call{value: amount};
    }
}
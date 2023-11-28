-- phpMyAdmin SQL Dump
-- version 4.6.6deb5
-- https://www.phpmyadmin.net/
--
-- Värd: localhost:3306
-- Tid vid skapande: 13 jan 2021 kl 15:48
-- Serverversion: 5.7.31-0ubuntu0.18.04.1
-- PHP-version: 7.2.24-0ubuntu0.18.04.6

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Databas: `portal`
--

-- --------------------------------------------------------

--
-- Tabellstruktur `bankidrequests`
--

CREATE TABLE `bankidrequests` (
  `id` int(11) NOT NULL,
  `license` varchar(100) NOT NULL,
  `method` varchar(25) NOT NULL,
  `pnr` varchar(25) NOT NULL,
  `response` varchar(1000) NOT NULL,
  `orderRef` varchar(100) NOT NULL,
  `collects` int(11) NOT NULL DEFAULT '0',
  `enviroment` varchar(15) NOT NULL,
  `creationdate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `lastcollect` datetime DEFAULT NULL,
  `lastcollectstatus` varchar(25) DEFAULT NULL,
  `userData` varchar(150) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Tabellstruktur `emails`
--

CREATE TABLE `emails` (
  `id` int(11) NOT NULL,
  `recipient` varchar(500) NOT NULL,
  `subject` varchar(100) NOT NULL,
  `message` varchar(2000) NOT NULL,
  `license` int(11) NOT NULL,
  `ip` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Tabellstruktur `licenses`
--

CREATE TABLE `licenses` (
  `ID` int(11) NOT NULL,
  `Product` varchar(255) NOT NULL,
  `Customer` varchar(255) NOT NULL,
  `Site` varchar(255) NOT NULL,
  `LicenseGUID` varchar(255) NOT NULL,
  `MachineGUID` varchar(255) NOT NULL DEFAULT '',
  `MaxCurrentUsers` int(11) NOT NULL DEFAULT '1',
  `DateAdded` date NOT NULL,
  `ExpirationDate` date NOT NULL,
  `AddedBy` varchar(50) NOT NULL,
  `SMS` tinyint(1) NOT NULL,
  `BankID` tinyint(4) NOT NULL DEFAULT '0',
  `BankIDLimit` int(11) DEFAULT NULL,
  `SMSLimit` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Tabellstruktur `log`
--

CREATE TABLE `log` (
  `id` int(11) NOT NULL,
  `action` varchar(355) NOT NULL,
  `object` varchar(255) NOT NULL,
  `action_by` varchar(255) NOT NULL,
  `from_ip` varchar(50) NOT NULL,
  `date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Tabellstruktur `products`
--

CREATE TABLE `products` (
  `ID` int(11) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `DisplayName` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Tabellstruktur `sms`
--

CREATE TABLE `sms` (
  `id` int(11) NOT NULL,
  `sender` varchar(50) NOT NULL,
  `reciever` varchar(300) NOT NULL,
  `licenseKey` varchar(300) NOT NULL,
  `message` varchar(350) NOT NULL,
  `date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `response` varchar(300) NOT NULL,
  `endpoint` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Tabellstruktur `users`
--

CREATE TABLE `users` (
  `id` int(11) NOT NULL,
  `username` varchar(50) NOT NULL,
  `password` varchar(255) NOT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `role` varchar(50) NOT NULL DEFAULT 'user'
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Index för dumpade tabeller
--

--
-- Index för tabell `bankidrequests`
--
ALTER TABLE `bankidrequests`
  ADD PRIMARY KEY (`id`);

--
-- Index för tabell `emails`
--
ALTER TABLE `emails`
  ADD PRIMARY KEY (`id`);

--
-- Index för tabell `licenses`
--
ALTER TABLE `licenses`
  ADD PRIMARY KEY (`ID`),
  ADD UNIQUE KEY `ID` (`ID`),
  ADD UNIQUE KEY `LicenseGUID` (`LicenseGUID`);

--
-- Index för tabell `log`
--
ALTER TABLE `log`
  ADD PRIMARY KEY (`id`);

--
-- Index för tabell `products`
--
ALTER TABLE `products`
  ADD PRIMARY KEY (`ID`),
  ADD UNIQUE KEY `name` (`Name`),
  ADD UNIQUE KEY `DisplayName` (`DisplayName`);

--
-- Index för tabell `sms`
--
ALTER TABLE `sms`
  ADD PRIMARY KEY (`id`);

--
-- Index för tabell `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `username` (`username`);

--
-- AUTO_INCREMENT för dumpade tabeller
--

--
-- AUTO_INCREMENT för tabell `bankidrequests`
--
ALTER TABLE `bankidrequests`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=145;
--
-- AUTO_INCREMENT för tabell `emails`
--
ALTER TABLE `emails`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;
--
-- AUTO_INCREMENT för tabell `licenses`
--
ALTER TABLE `licenses`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=121;
--
-- AUTO_INCREMENT för tabell `log`
--
ALTER TABLE `log`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=150;
--
-- AUTO_INCREMENT för tabell `products`
--
ALTER TABLE `products`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;
--
-- AUTO_INCREMENT för tabell `sms`
--
ALTER TABLE `sms`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=55904;
--
-- AUTO_INCREMENT för tabell `users`
--
ALTER TABLE `users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

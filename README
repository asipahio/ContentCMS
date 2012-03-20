ContentCMS
=============

A small content editor using MVC3.

Installation
-------

### DB structure and Creation
pageID - Int NOT NULL IDENTITY
pageURL - VarChar(100)
pageTitle - VarChar(100)
pageContent - NText
lastUpdate - DateTime
isPublished - Bit NOT NULL
parent - Int NOT NULL
isContent - Bit NOT NULL
isStatic - Bit NOT NULL
author - UniqueIdentifier
pageOrder - Int

You can run the following SQL line to create the database:

	CREATE TABLE pages(
	pageID INT NOT NULL IDENTITY(1,1),
	pageURL VarChar(100),
	pageTitle VarChar(100),
	pageContent NText,
	lastUpdate DateTime,
	isPublished Bit NOT NULL,
	parent Int NOT NULL,
	isContent Bit NOT NULL,
	isStatic Bit NOT NULL,
	author UniqueIdentifier,
	pageOrder Int,
	PRIMARY KEY(pageID))

Also the following 3 lines to create the dummy content to see the menus populated:

	INSERT INTO pages (pageURL, pageTitle, pageContent, lastUpdate, isPublished, parent, isContent, isStatic, author, pageOrder)
	VALUES ('/home/', 'Home', NULL, '03/16/2012', true, 0, true, true, NULL,0);

	INSERT INTO pages (pageURL, pageTitle, pageContent, lastUpdate, isPublished, parent, isContent, isStatic, author, pageOrder)
	VALUES ('/about/', 'About Us', 'Content for about us page', '03/16/2012', true, 0, true, false, NULL, 1);

	INSERT INTO pages (pageURL, pageTitle, pageContent, lastUpdate, isPublished, parent, isContent, isStatic, author, pageOrder)
	VALUES ('/about/demo/', 'Demo', 'Content for demo page under the about us', '03/16/2012', true, 2, true, false, NULL, 1);


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


Usage
-------

All the pages you create within the database, if they are dynamic pages will be grouped under `/p/`. 

### Static Page
If you want to create a static page, that uses a Controller, then set the `isStatic` flag to true for that record. 

### Just a Menu item without a link
If you want to just create an item in the menu but not link to anywhere, then set `isContent` flag to false for that record.


Other 
-------
I created an `author` field, which I am planning on tying to asp.net's membership tables and creating an admin page where users can add new content and their GUID will be 
recorded in this field. 

You can unpublish a page by just setting the isPublished flag to false.
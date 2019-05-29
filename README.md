# MTGA Pro Tracker
MTGA Pro Tracker is an advanced Magic the Gathering: Arena tracking tool that automatically uploads collection, decks, battles, draft and inventory from your game client to our server. No manual collection input, no manual uploads. New cards, battles & drafts are added immediately based on in-game events.

## What can I do with MTGA Pro Tracker?
* Real Time Collection & Decks Import.
* Share Progress and Wins/Losses.
* Track your Wildcards, Boosters and Vault.
* Deckbuild using your current collection.
* Get in-game help with matches and draft using the Overlay tool.

## Hotkeys
* Alt+Q to see your cards.
* Alt+W to see opponent.
* Alt+~ to collapse/expand the overlay.
* Alt+O (not zero, letter O) - to completely hide overlay or show it back.

## INSTALLATION STEP-BY-STEP GUIDE
IMPORTANT! Program needs .NET 4.6.2 or higher to work. If app doesn't start on your PC (no interface shows up after you launch EXE) you should install .NET from here!

* Download MTGA Pro Tracker from our Github repo Releases section (https://github.com/Razviar/mtga-pro-tracker/releases ). Antivirus software (including Windows Defender) could be alarmed because of the app's ability to upload data to remote server and automatic update feature. So if Antivirus is alarmed, you should add this App to exceptions.
* Unpack it and launch MSI Installer.
* Get your token from widget on this page: https://mtgarena.pro/mtga-pro-tracker/ You must be registered user to get token!
* Copy this digit-letter code and paste it to input field of Tracker app.
* Right after you paste the code, your username (from MTGArena.pro website) will be displayed in the app window. Tokens are being stored, token authorization happens just once. Next time you launch the app, it will remember youyr token and username.
That's it! If  you've launched MTGA on your PC before, upload will start instantly. When new cards are added, those will be updated as well.

## Windows protected your PC?
When you get mesage "Windows protected your PC", you should click "More Info..." and "Run Anyway". Defender gets alarmed because application is not signed. After several scans, it will calm down and let you use Tracker. 

## TRACKER IS NOT UPDATING?
App crashes or not starting? No recent updates uploaded? Follow steps:

1. Make sure you have latest version. It's updated to be compatible with latest MTGA version.
2. Be sure that you have .NET 4.6.2 or higher.

3. If you are able to see app interface, just click FULL RESET button, then re-input new token.
4. Try to reboot your PC.
5. Check if antiviruses or firewalls are blocking app traffic, add app to exception.

## Changelog
v.1.6.9 released 26/05/2019 (Catch-Up Fix)
* Issues with old logs uploads and syncing of data from gaming sessions without Tracker was running are fixed.

v.1.6.8 released 26/05/2019 (Quickfix)
* Scan Old Logs issue resolved
* Warning dialog about another trackers running disabled
* Improved stability of long logs reading

v.1.6.7 released 26/05/2019 (Stability Update)
* "Can't parse date" issue fixed
* Implemented more simplistic yet more stable data uploads method
* Tiny bug fixes

v.1.6.4 released 05/03/2019 (Maintenance Release)
* Starting to fix bugs, caught by monitoring system.

v.1.6.3 released 04/03/2019 (Maintenance Release)
* Significantly improved crashes tracking, which will let us to improve stability in the next releases.

v.1.6.1 released 19/02/2019 (Maintenance Release)
* Overlay bugs fixes according to users reports. Now decks are rendering properly after first match played.
* Option to disable hotkeys (if they overlap with other software you use).
* Better data upload: now all events happening in singe second time span will be processed correctly (it was only the latest event from * the second processed before).
* Many tiny fixes, leading to better stability.
* Changed "other trackers policy". Now app just warns you about the risks, and warns you once per run. You can use several trackers at your own risk. 

v.1.6.0 released 04/02/2019 (In-game Overlay fixes)
* Overlay bugs fixes according to users reports.
* New settings, giving even more customization to overlay.

v.1.5.9 released 31/01/2019 (In-game Overlay)
* Added in-game overlay with 3 modes: matches, decks and draft helper.
* Added detailed settings for overlay.
* Improved app interface.
* Code signed. Now antiviruses will not get triggered with our software.
* To enable overlay just click respective button in the app interface.

v.1.5.3 released 03/01/2019 (New Year Update)
* Notifications from developers in the app; you will not need to look for information about tracking issues on Twitter or Reddit. 
* Better update notifications. You will see the update button right after update is out.
* More stable match tracking. Opponent's cards from previous matches will not persist to the next match. 
* Remote restart; the tracker will now restart itself when needed. New features will be available without action required from user. 
* Protection from log overgrowing. If your MTGA log grows huge, app deals with it properly. 
* More stable uploads. Risk of tracker get stuck so you need manual full reset is minimal.
* In-game time counter; you will now how much time you spent on MTGA on daily basis. 

v.1.5.2 released 23/11/2018 (No initial upload)
* Fix of unwanted behaviour when app uploads old data from the log on every startup, even if that data already was uploaded.

v.1.5.1 released 21/11/2018 (Better UX)
* Data upload progress bar shows process of big log uploads.
* Buttons are now disabled when they do nothing or shouldn't be pressed.
* Manual Reset now cleans up AppData folder; no need to manually clean it.

v.1.5.0 released 21/11/2018 (Catch-up Mode)
* NEW: introduced manual re-sync and old logs reading.
* Improved data delivery on server leading to more stable data uploads.
* Tracker now gets all battles from latest log, even if it wasn't running.
* Fixed time. Users will now have their time recorded according to their timezones.
* Backlog: still no direct challenges decks detection. This feature will be added later. 

v.1.4.8 released 15/11/2018 (Hotfix)
* Fixed possible issues with collections and decks sync.
* Some interface improvements and more stability.

v.1.4.6 released 11/11/2018 (Hotfix)
* Hotfix: Server protection from upload-induced DDOS attacks.
* Hotfix: Log parser for German users.
* Numerous tiny bug fixes.

v.1.4.3 released 09/11/2018 (Better data delivery system):
* Data transmission format from client to server reworked in order to provide more stability and reduce server load.

v.1.3.7 released 09/11/2018 (Hopefully final hotfix):
* Fixed error with "Client needs update" message.
* Fixed date/time bugs.
* Fixed globalization issues with different date/time formats.
* Various tweaks and improvements.

v.1.3.5 released 07/11/2018 (Hotfix):
* Fixed client-side data storage system.
* Added nice banner.

v.1.3.4 released 07/11/2018 (Big Update!):
* Data extraction from latest log, even if Tracker was not running. Important: if the game was restarted several times, only data from the latest session without tracker will survive.
* Better log parsing system which paves the path to full match replay function (not implemented yet in MTGArena.pro, but client-side is fully ready).
* Upload success control and local data storage during server downtime; match tracking will not be lost due to server maintenance.
* Fixed multiple accounts; this function is working properly.
* Stability improvements. The app now restarts itself if it crashes; no data will be lost.
* Logging improvements. Now we will be able to monitor bugs more closely to fix them faster.
* Better integration with MTGA process.
* Better Windows 7 support.

v.1.3.2 released 03/10/2018 (Google Cloud):
* Server load moved to Google Cloud.

v.1.3.1 released 01/10/2018 (Server Load Reduce and Stability Fixes):
* Optimizations to reduce server load.
* Added MTGA player nicks recognition.
* Fixed several bugs and improved stability.

v.1.3.0 released 31/08/2018 (Multiaccount Fix & better bug tracker embeded):
* Fixed Multiaccount function; when you change accounts in a game, Tracker will switch tokens or request a new token.
* Better bugtracking system will help me to intercept tiny glitches in tracking process and make respective fixes.

v.1.2.9 released 17/08/2018 (MTGA v.818_646046 compatibility):
* Updated log format.

v.1.2.7 released 08/08/2018 (Dynamic Monitoring):
* Now you don't need to update app in order to get new tracking tools. App loads monitoring patterns dynamically from server.
* Various bug fixes.

v.1.2.6 released 06/08/2018 (Maintenance Release):
* Various bug fixes.
* Resolved compatibility issues with Win7 & Win8.

v.1.2.5 released 03/08/2018 (Optimization and fixes):
* Better MSI installer; no cab-files and directory selector added.
* Better string processing (thanks to u/Spongman).
* Improved log processing; Tracker will not re-scan the whole log on startup if no changes have occurred.

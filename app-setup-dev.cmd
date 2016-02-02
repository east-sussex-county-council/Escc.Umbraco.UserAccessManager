@echo off
:: Run Powershell script from a .bat file 
:: http://blog.danskingdom.com/allow-others-to-run-your-powershell-scripts-from-a-batch-file-they-will-love-you-for-it/
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& '%~dpn0.ps1';
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Dhgms.AspNetCoreContrib.Example.WebSite.Features.Excel
{
    public static class SpreadsheetDocumentHelper
    {
        public static SpreadsheetDocument GetWorkbookSpreadSheetDocument(
            Stream stream,
            IList<(string Name, Action<Sheet, WorksheetPart> Actor)> sheetActors)
        {
            var spreadsheetDocument = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            var workbookPart = spreadsheetDocument.AddWorkbookPart();
            var workbook = new Workbook();
            workbookPart.Workbook = workbook;

            // Add a WorksheetPart to the WorkbookPart.
            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

            var sheetData = new SheetData();
            var worksheet = new Worksheet(sheetData);
            worksheetPart.Worksheet = worksheet;

            var sheets = workbook.AppendChild(new Sheets());

            var workSheetPartId = workbookPart.GetIdOfPart(worksheetPart);

            if (sheetActors?.Count > 0)
            {
                AddWorkSheets(sheetActors, workSheetPartId, worksheetPart, sheets);
            }

            return spreadsheetDocument;
        }

        private static void AddWorkSheets(
            IList<(string Name, Action<Sheet, WorksheetPart> Actor)> sheetActors,
            string workSheetPartId,
            WorksheetPart worksheetPart,
            Sheets sheets)
        {
            var sheetsToAdd = new List<Sheet>(sheetActors.Count);
            for (int i = 0; i < sheetActors.Count; i++)
            {
                var current = sheetActors[i];
                var name = current.Name;
                var actor = current.Actor;

                var sheet = new Sheet
                {
                    Id = workSheetPartId,
                    SheetId = (uint)i + 1,
                    Name = name,
                };

                actor(sheet, worksheetPart);
                sheetsToAdd.Add(sheet);
            }

            sheets.Append(sheetsToAdd);
        }
    }
}

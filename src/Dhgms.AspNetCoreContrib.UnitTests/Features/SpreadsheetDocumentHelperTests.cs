﻿// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Dhgms.AspNetCoreContrib.App.Features.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Dhgms.AspNetCoreContrib.UnitTests.Features
{
    /// <summary>
    /// Unit tests for the Excel Spreadsheet document helper.
    /// </summary>
    public static class SpreadsheetDocumentHelperTests
    {
        /// <summary>
        /// Unit tests for workbook generation.
        /// </summary>
        public sealed class GetWorkbookSpreadSheetDocumentMethod : Foundatio.Logging.Xunit.TestWithLoggingBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GetWorkbookSpreadSheetDocumentMethod"/> class.
            /// </summary>
            /// <param name="output">XUnit Test Output helper.</param>
            public GetWorkbookSpreadSheetDocumentMethod(ITestOutputHelper output)
                : base(output)
            {
            }

            /// <summary>
            /// Tests to ensure a spreadsheet is returned.
            /// </summary>
            [Fact]
            public void ReturnsSpreadSheet()
            {
                using (var stream = new MemoryStream())
                {
                    var sheetActors = new List<(string Name, Action<Sheet, WorksheetPart> Actor)>();
                    sheetActors.Add(("Sheet1", CreateSheet1));
                    var workbook = SpreadsheetDocumentHelper.GetWorkbookSpreadSheetDocument(stream, sheetActors);

                    Assert.NotNull(workbook);

                    workbook.Save();
                    workbook.Close();

                    var buffer = stream.ToArray();
                    var stringOutput = Encoding.UTF8.GetString(buffer);
                    _logger.LogDebug(stringOutput);
                }
            }

            private static void CreateSheet1(Sheet sheet, WorksheetPart worksheetPart)
            {
                uint currentRow = 1;
                var titleCell = InsertCellInWorksheet("A", currentRow, worksheetPart);
                titleCell.CellValue = new CellValue("Title");
                currentRow++;
            }

            // Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet.
            // If the cell already exists, returns it.
            private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
            {
                var worksheet = worksheetPart.Worksheet;
                var sheetData = worksheet.GetFirstChild<SheetData>();
                var cellReference = columnName + rowIndex;

                // If the worksheet does not contain a row with the specified row index, insert one.
                var row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
                if (row != null)
                {
                    var cell = row.Elements<Cell>().FirstOrDefault(c => c.CellReference.Value == cellReference);
                    if (cell != null)
                    {
                        return cell;
                    }
                }
                else
                {
                    row = new Row { RowIndex = rowIndex };
                    sheetData.Append(row);
                }

                Cell refCell = null;
                foreach (var cell in row.Elements<Cell>())
                {
                    if (cell.CellReference.Value.Length == cellReference.Length)
                    {
                        if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                        {
                            refCell = cell;
                            break;
                        }
                    }
                }

                var newCell = new Cell { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }
    }
}

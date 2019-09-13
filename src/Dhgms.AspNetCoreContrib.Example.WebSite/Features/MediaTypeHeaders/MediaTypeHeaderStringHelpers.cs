﻿namespace Dhgms.AspNetCoreContrib.Example.WebSite.Features.MediaTypeHeaders
{
    public static class MediaTypeHeaderStringHelpers
    {
        //TODO : use roslyn to generate these from Microsoft.AspNet.StaticFiles.FileExtensionContentTypeProvider 

        public static string ApplicationPdf => "application/pdf";

        public static string ApplicationVndOpenXmlFormatsOfficeDocumentSpreadsheetMlSheet =>
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    }
}

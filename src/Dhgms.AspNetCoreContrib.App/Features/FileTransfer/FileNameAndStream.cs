﻿// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dhgms.AspNetCoreContrib.Example.WebSite.Features.FileTransfer
{
    public sealed class FileNameAndStream
    {
        public string FileName { get; set; }
        public Stream FileStream { get; set; }
    }
}

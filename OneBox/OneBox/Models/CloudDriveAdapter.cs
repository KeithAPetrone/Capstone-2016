﻿using DropboxRestAPI.Models.Core;
using Google.Apis.Drive.v2.Data;
using System;

namespace OneBox.Models
{
    public class CloudDriveAdapter
    {
        File googleFile;
        MetaData dropboxFile;

        public string Title { get; }
        public string Extension { get; }


        public CloudDriveAdapter(File file)
        {
            this.googleFile = file;
            this.dropboxFile = null;

            this.Title = googleFile.Title;
            this.Extension = googleFile.FileExtension;
        }

        public CloudDriveAdapter(MetaData file)
        {
            this.dropboxFile = file;
            this.googleFile = null;

            this.Title = dropboxFile.Name;
            this.Extension = dropboxFile.Extension;
        }

        public Tuple<File, MetaData> GetFile()
        {
            return new Tuple<File, MetaData>(this.googleFile, this.dropboxFile);
        }
    }
}
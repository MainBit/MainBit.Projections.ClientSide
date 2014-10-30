using MainBit.Projections.ClientSide.Models;
using Orchard.Data.Migration;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainBit.Projections.ClientSide
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            ContentDefinitionManager.AlterPartDefinition(typeof(ClientSideProjectionPart).Name, part => part
                .Attachable()
                );

            SchemaBuilder.CreateTable("ClientSideProjectionPartRecord",
                table => table
                    .ContentPartRecord()
                    .Column<string>("Data", c => c.Unlimited())
                    .Column<string>("PresetQueryString")
                    .Column<int>("Items")
                    .Column<int>("Skip")
                    .Column<int>("MaxItems")
                    .Column<string>("PagerSuffix", c => c.WithLength(255))
                    .Column<bool>("DisplayPager")
                    .Column<int>("QueryPartRecord_id")
                );

            return 1;
        }
    }
}
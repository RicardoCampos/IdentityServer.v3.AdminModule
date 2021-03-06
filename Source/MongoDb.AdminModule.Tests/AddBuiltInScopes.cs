﻿/*
 * Copyright 2014, 2015 James Geall
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System.Linq;
using System.Management.Automation;
using IdentityServer.Core.MongoDb;
using IdentityServer.MongoDb.AdminModule;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace MongoDb.AdminModule.Tests
{
    public class AddBuiltInScopes : IUseFixture<PowershellAdminModuleFixture>
    {
        private IScopeStore _scopeStore;
        private PowerShell _ps;
        private PowershellAdminModuleFixture _data;

        [Fact]
        public void VerifyAllBuiltInScopes()
        {
            _ps.Invoke();
            Assert.Null(_data.GetPowershellErrors());
            Assert.Equal(
                ReadScopes.BuiltInScopes()
                    .OrderBy(x => x.Name)
                    .Select(TestData.ToTestableString), 
                _scopeStore.GetScopesAsync(false).Result
                    .OrderBy(x=>x.Name)
                    .Select(TestData.ToTestableString)
                );
        }

        public void SetFixture(PowershellAdminModuleFixture data)
        {
            _data = data;
            _ps = data.PowerShell;
            var script = data.LoadScript(this);
            var database = data.Database;
            _ps.AddScript(script).AddParameter("Database", database);
            _scopeStore = data.Factory.Resolve<IScopeStore>();
            var adminService = data.Factory.Resolve<IAdminService>();
            adminService.CreateDatabase();
        }
    }
}

﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NXA.SC.Caas.Services.Token;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using NXA.SC.Caas.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;

namespace nxa_sc_caas.UnitTests
{
    [TestClass]
    public class UnitTest_ApiTokenAuth
    {
        [TestMethod]
        public void ApiTokenValid()
        {
            var db = TokenCreator.AddTokensToDb();
            var service = TokenCreator.CreateTokenService(db.Result);
            var tokenStr = "b199ede5-107a-4cda-b475-7f55569daa05";
            var valid = service.TokenIsValid(tokenStr);

            Assert.IsTrue(valid);
            db.Result.Database.EnsureDeleted();
        }

        [TestMethod]
        public void ApiTokenInValid()
        {
            var db = TokenCreator.AddTokensToDb();
            var service = TokenCreator.CreateTokenService(db.Result);
            var tokenStr = "490b9e4e-66eb-4f8c-8166-c08d7056fdf0";
            var valid = service.TokenIsValid(tokenStr);

            Assert.IsFalse(valid);
            db.Result.Database.EnsureDeleted();
        }

        [TestMethod]
        public void ApiTokenNotInDb()
        {
            var db = TokenCreator.AddTokensToDb();
            var service = TokenCreator.CreateTokenService(db.Result);
            var tokenStr = "190b9e4e-66eb-4f8c-8166-c08d7056fdf0";
            var valid = service.TokenIsValid(tokenStr);

            Assert.IsFalse(valid);
            db.Result.Database.EnsureDeleted();
        }

        [TestMethod]
        public void ApiTokenExpired()
        {
            var db = TokenCreator.AddTokensToDb();
            var service = TokenCreator.CreateTokenService(db.Result);
            var tokenStr = "2d713fbf-1c0b-4a55-b64e-d2d16905908e";
            var valid = service.TokenIsValid(tokenStr);

            Assert.IsFalse(valid);
            db.Result.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task ApiTokenHandler_AuthenticateValidToken()
        {
            var db = TokenCreator.AddTokensToDb();
            var tokenHandler = TokenCreator.CreateTokenHandler(db.Result);
            var context = new DefaultHttpContext();
            context.Request.Headers["Token"] = "b199ede5-107a-4cda-b475-7f55569daa05";
            await tokenHandler.InitializeAsync(new AuthenticationScheme(TokenAuthOptions.DefaultScemeName, null, typeof(ApiTokenHandler)), context);
            var result = await tokenHandler.AuthenticateAsync();

            Assert.IsTrue(result.Succeeded);
            db.Result.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task ApiTokenHandler_DontAuthenticateInvalidToken()
        {
            var db = TokenCreator.AddTokensToDb();
            var tokenHandler = TokenCreator.CreateTokenHandler(db.Result);
            var context = new DefaultHttpContext();
            context.Request.Headers["Token"] = "c199ede5-107a-4cda-b475-7f55569daa05";
            await tokenHandler.InitializeAsync(new AuthenticationScheme(TokenAuthOptions.DefaultScemeName, null, typeof(ApiTokenHandler)), context);
            var result = await tokenHandler.AuthenticateAsync();

            Assert.IsFalse(result.Succeeded);
            db.Result.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task ApiTokenHandler_DontAuthenticateEmptyHeader()
        {
            var db = TokenCreator.AddTokensToDb();
            var tokenHandler = TokenCreator.CreateTokenHandler(db.Result);
            var context = new DefaultHttpContext();
            await tokenHandler.InitializeAsync(new AuthenticationScheme(TokenAuthOptions.DefaultScemeName, null, typeof(ApiTokenHandler)), context);
            var result = await tokenHandler.AuthenticateAsync();

            Assert.IsFalse(result.Succeeded);
            db.Result.Database.EnsureDeleted();
        }
    }
}

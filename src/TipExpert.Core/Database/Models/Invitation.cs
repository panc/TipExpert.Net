﻿using MongoDB.Bson;

namespace TipExpert.Core
{
    public class Invitation
    {
        public ObjectId Id { get; set; }

        public ObjectId GameId { get; set; }

        public ObjectId UserId{ get; set; }

        public string Email { get; set; }
    }
}
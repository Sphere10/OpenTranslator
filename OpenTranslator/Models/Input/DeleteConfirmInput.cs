﻿namespace OpenTranslator.Models.Input
{
    public class DeleteConfirmInput
    {
		public int Id { get; set; }

        public string TextId { get; set; }

        public string Message { get; set; }

        public string GridId { get; set; }
    }
}
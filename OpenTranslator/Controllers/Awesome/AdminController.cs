﻿using OpenTranslator.Data;
using OpenTranslator.Utils;
using OpenTranslator.Models.Input;
using Omu.AwesomeMvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using OpenTranslator.Repostitory;
using System.Web.Mvc;
using System.Linq.Expressions;
using OpenTranslator.Models;
using System.Collections;
using System.Dynamic;
using System.Net;
using Omu.Awem.Helpers;

namespace OpenTranslator.Controllers.Awesome
{
	public class AdminController : Controller
	{
		public StringTranslationEntities entities = new StringTranslationEntities();

		private ITranslation ITranslation;
		private ILanguages ILanguages;
		private ITranslationLog ITranslation_Log;
		private ITranslationArchive ITranslationArchive;
		private IVotes IVotes;
		private ITranslationMode ITranslationMode;
		public AdminController()
		{
			this.ITranslation = new TranslationRepository(new StringTranslationEntities());
			this.ILanguages = new LanguageRepository(new StringTranslationEntities());
			this.ITranslation_Log = new TranslationLogRepository(new StringTranslationEntities());
			this.ITranslationArchive = new TranslationArchiveReopsitory(new StringTranslationEntities());
			this.IVotes = new VotesRepository(new StringTranslationEntities());
			this.ITranslationMode = new TranslationModeRepository(new StringTranslationEntities());
		}
		public ActionResult GetEnumItems()
		{
			string CurrentURL = Request.Url.AbsoluteUri;

			var type = typeof(Models.Translationmodes);
			var items = Enum.GetValues(type).Cast<int>().Select(o => new KeyContent(o, Enum.GetName(type, o))).ToList();
			return Json(items);
		}

		public ActionResult GetColumnsItems(string[] columns)
		{
			List<Language> list = new List<Language>();
			if(Session["SelectedColumns"] == null)
			{
				list = ILanguages.GetLanguages().ToList();	
			}
			var column = new List<String>();
			for (int i = 0; i < list.Count; i++)
			{
				if (!list[i].LanguageCode.Equals("en"))
					column.Add(list[i].LanguageName.ToString());
			}
			var col = column.ToArray();
			columns = columns ?? col;

			return Json(columns.Select(o => new KeyContent(o, o)));
		}

		public ActionResult Index()
		{
			return View();
		}
		private static object MapToGridModel(GridArrayRow o)
		{
			return
				new
				{
					o.TextId,
					o.Values,

				};
		}

		public GridFormatData Gridformat()
		{
			var languages = ILanguages.GetLanguages().Select(x => x.LanguageCode).Distinct();
			var query = from r in ITranslation.GetTranslation().Where(x => x.OfficialBoolean == true)
						group r by r.TextId into nameGroup
						select new
						{
							Name = nameGroup.Key,
							Values = from lang in languages
									 join ng in nameGroup
										  on lang equals ng.LanguageCode into languageGroup
									 select new
									 {
										 Column = lang,
										 Value = languageGroup.Any() ?
												 languageGroup.First().Translated_Text : null
									 }
						};

			DataTable table = new DataTable();
			var languagesList = ILanguages.GetLanguages().Select(x => x.LanguageName).Distinct();
			table.Columns.Add("TextId");  // first column
			foreach (var language in languagesList)
				table.Columns.Add(language); // columns for each language
			List<string> items = new List<string>();
			foreach (var key in query)
			{
				var row = table.NewRow();
				items = key.Values.Select(v => v.Value).ToList(); // data for columns
				items.Insert(0, key.Name); // data for first column
				row.ItemArray = items.ToArray();
				table.Rows.Add(row);
			}
			var tableList = table.AsEnumerable().AsQueryable();
			tableList.ToArray().AsQueryable();
			List<string[]> gridDataList = new List<string[]>();
			string[] columnNames = table.Columns.Cast<DataColumn>()
								 .Select(x => x.ColumnName)
								 .ToArray();

			gridDataList.Add(columnNames);
			foreach (var listData in tableList)
			{
				gridDataList.Add(listData.ItemArray.Select(x => x.ToString()).Cast<string>().ToArray());
			}

			var columns = new List<Column>();
			columns.Add(new Column { Header = "TextId", ClientFormat = ".TextId" });

			for (var i = 1; i < gridDataList[0].Length; i++)
			{
				columns.Add(new Column { Header = gridDataList[0][i], ClientFormatFunc = "getVal(" + i + ", '" + gridDataList[0][i] + "')" });
			}

			columns.Add(new Column { ClientFormat = GridUtils.DeleteFormatForGrid("OriginalTextGrid", "TextId"), Width = 50 });

			var gridItems = new List<GridArrayRow>();
			for (var i = 1; i < gridDataList.Count; i++)
			{
				gridItems.Add(new GridArrayRow { TextId = gridDataList[i][0], Values = gridDataList[i] });
			}

			GridFormatData data = new GridFormatData();
			data.GridRows = gridItems;
			data.GridColumn = columns;

			return data;
		}


		public ActionResult OriginalTextGridGetItems(GridParams g, string[] selectedColumns, bool? choosingColumns, string UserType = "Admin")
		{

			var GridData = this.Gridformat();
			var columns = GridData.GridColumn.ToArray();
			if(selectedColumns != null)
			{
				Session["SelectedColumns"] = selectedColumns;
				//Response.Cookies["SelectedColumns"].Value = selectedColumns.ToString() ;
				//Response.Cookies["SelectedColumns"].Expires = DateTime.Now.AddMonths(1);
			}
				

			if(Session["SelectedColumns"] != null)
			{
				selectedColumns = (string[])Session["SelectedColumns"];
				//selectedColumns = Response.Cookies["SelectedColumns"].Value;
				choosingColumns = true;

			}

			if (UserType.Equals("User"))
			{
				columns = columns.Take(columns.Count() - 1).ToArray();
			}
			g.Columns = columns;

			var baseColumns = new[] { "TextId", "English" };

			//first load
			if (g.Columns.Length == 0)
			{
				g.Columns = columns.ToArray();
			}

			if (choosingColumns.HasValue)
			{
				selectedColumns = selectedColumns ?? new string[] { };

				//make sure we always have Id and Person columns
				selectedColumns = selectedColumns.Union(baseColumns).ToArray();

				var currectColumns = g.Columns.ToList();

				//remove unselected columns
				currectColumns = currectColumns.Where(o => selectedColumns.Contains(o.Header)).ToList();

				//add missing columns
				var missingColumns = selectedColumns.Except(currectColumns.Select(o => o.Header)).ToArray();

				currectColumns.AddRange(columns.Where(o => missingColumns.Contains(o.Header)));

				g.Columns = currectColumns.ToArray();
			}

			var model = new GridModelBuilder<GridArrayRow>(GridData.GridRows.AsQueryable(), g)
			{
				Key = "TextId",
				GetItem = () => GridData.GridRows.Single(x => x.TextId == g.Key),
				Map = MapToGridModel

			}.Build();
			return Json(model);

		}


		public ActionResult Create()
		{
			return PartialView();
		}

		[HttpPost]
		public ActionResult Create(AdminInput input)
		{
			if (!ModelState.IsValid)
			{
				return PartialView(input);
			}
			if (this.doesTextIdExist(input) == true)
			{
				return PartialView(input);

			}
			else
			{
				input.TextId = input.TextId;
				Text text = new Text();
				text.TextId = input.TextId;
				text.System = true;

				Translation translation = new Translation();
				translation.LanguageCode = input.LanguageCode;
				translation.TextId = text.TextId;
				translation.Translated_Text = input.Text;
				translation.OfficialBoolean = true;
				translation.Votes = 0;

				TranslationLog translation_log = new TranslationLog();
				translation_log.TextId = input.TextId;
				translation_log.System_Date = DateTime.Now;
				translation_log.LanguageCode = input.LanguageCode;
				translation_log.Translated_Text = input.Text;

				ITranslation.InsertTextTranslation(text, translation, translation_log);

				// returning the key to call grid.api.update
				var data = this.Gridformat();
				var rowData = data.GridRows.Where(x => x.TextId == input.TextId).FirstOrDefault();
				return Json(MapToGridModel(rowData));
			}
		}

		[HttpPost]
		public bool doesTextIdExist(AdminInput input)
		{

			var id = Convert.ToDecimal(input.Id);
			if (input.Id == 0)
			{
				var text = ITranslation.GetText().Where(x => x.TextId == input.TextId).FirstOrDefault();
				if (text != null)
				{
					ViewBag.errormsg = "TextId already exist.";
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				var text = ITranslation.GetText().Where(x => x.TextId == input.TextId && x.Id != id).FirstOrDefault();
				if (text != null)
				{
					ViewBag.errormsg = "TextId already exist.";
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		public ActionResult TranslationItems(GridParams g, string TextId, string LanguageCode)
		{
			var items = ITranslation.GetTranslationLogByCode(TextId, LanguageCode).AsQueryable();
			var key = Convert.ToInt32(g.Key);
			var model = new GridModelBuilder<Translation>(items.AsQueryable(), g)
			{
				Key = "Id",
				GetItem = () => items.Single(x => x.Id == key)
			}.Build();
			return Json(model);

		}
		[HttpPost]
		public ActionResult VoteCount(string vote, string textid, string text, string code)
		{
			var getMode= ITranslationMode.GetTranslationModeByID(textid,code);

			if(getMode.Mode==2)
			{
				return PartialView();
			}
			else
			{
			if (Request.Cookies["BrowserId"] == null)
			{
				Guid id = Guid.NewGuid();
				Response.Cookies["BrowserId"].Value = id.ToString();
				Response.Cookies["BrowserId"].Expires = DateTime.Now.AddMonths(1);
			}
			
			Guid cookieValue =  Guid.Parse(Request.Cookies["BrowserId"].Value);
			string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
			string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();


			//var session = HttpContext.Session.SessionID;
			var voteData = IVotes.GetVoteByCookieID(cookieValue);

			var items = ITranslation.GetTranslationLogByCode(textid, code).ToList();
			var list = items;
			var data = items.Where(x => x.Translated_Text == text).FirstOrDefault();
			if (voteData == null)
			{
				data.Votes = data.Votes + 1;
				Vote addVote = new Vote();
				addVote.CookieID = cookieValue;
				addVote.Translation_Id = data.Id;
				addVote.IP = myIP;
				IVotes.InsertVote(addVote);
				
			}
			else
			{
				var voteList = IVotes.GetVoteList(cookieValue);
				var voteResult = voteList.Where(x => items.Any(p => p.Id == x.Translation_Id));
				foreach (var item in items)
				{
					var id = Convert.ToInt32(item.Id);
					var updateVote= IVotes.GetVoteBytranslationId(id,cookieValue);
					//
					if (updateVote != null)
					{
						item.Votes = item.Votes - 1;
						data.Votes = data.Votes + 1;
						ITranslation.UpdateTranslation(item);
						IVotes.RemoveVote(updateVote.Id);
						Vote addVote = new Vote();
						addVote.CookieID = cookieValue;
						addVote.Translation_Id = data.Id;
						addVote.IP = myIP;
						IVotes.InsertVote(addVote);


					}

				}
				if (voteResult.Count() == 0)
				{
					data.Votes = data.Votes + 1;
					Vote addVote = new Vote();
					addVote.CookieID = cookieValue;
					addVote.Translation_Id = data.Id;
					addVote.IP = myIP;
					IVotes.InsertVote(addVote);
				}
			}
			if(getMode.Mode==1)
                {
                    ITranslation.UpdateTranslation(data);
                    return Json(data);
                }
                else
                {
					foreach (var item in items)
					{
						item.OfficialBoolean = false;
					}
					var maxVote = list.Max(s => s.Votes);

					var mapdata = list.Where(x => x.Votes == maxVote).ToList();
					if (mapdata.Count > 1)
					{
						data.OfficialBoolean = true;
					}
					else
					{
						var setdata = list.Where(x => x.Votes == maxVote).FirstOrDefault();
						setdata.OfficialBoolean = true;
						setdata.Votes = maxVote;

						ITranslation.UpdateTranslation(setdata);

					}
					ITranslation.UpdateTranslation(data);

					return Json(data);
				}

			}
		}


		public ActionResult Edit(string TextId, string code, string UserType="Admin")
		{
			var viewname = "";
			if(UserType.Equals("Admin"))
			{
				viewname = "EditTranslation";
			}
			else
			{
				viewname = "../User/EditTranslation";
			}
			Language languages = ILanguages.GetLanguages().Where(x => x.LanguageName == code).FirstOrDefault();
			Translation translation = ITranslation.GetTranslation().Where(x => x.TextId == TextId && x.LanguageCode == languages.LanguageCode &&x.OfficialBoolean==true).FirstOrDefault();
			var translationMode= ITranslationMode.GetTranslationModeByID(TextId,languages.LanguageCode);

			if (translation == null)
			{
				return PartialView(viewname, new TranslationInput { TextId = TextId, TranslationText = null, LanguageCode = languages.LanguageCode , ModeOfTranslation=0});
			}
			else
			{
				var id = Convert.ToInt32(translation.Id.ToString());
				if(translationMode==null)
				{
					return PartialView(viewname, new TranslationInput { TextId = TextId, TranslationText = translation.Translated_Text, LanguageCode = languages.LanguageCode, Id = id, ModeOfTranslation=0});
				}
				else
				{
					return PartialView(viewname, new TranslationInput { TextId = TextId, TranslationText = translation.Translated_Text, LanguageCode = languages.LanguageCode, Id = id, ModeOfTranslation=translationMode.Mode});
				}
			}

		}

		[HttpPost]
		public ActionResult Edit(TranslationInput input)
		{
			if (!ModelState.IsValid)
			{
				return PartialView("EditTranslation", input);
			}
			var updateMode= ITranslationMode.GetTranslationModeByID(input.TextId,input.LanguageCode);
			if(updateMode== null)
			{
				TranslationMode mode = new TranslationMode();
				mode.TextId=input.TextId;
				mode.LanguageCode=input.LanguageCode;
				mode.Mode=input.ModeOfTranslation;
				ITranslationMode.InsertTranslationMode(mode);
				if(input.ModeOfTranslation==2)
				{
					var griddata = this.Gridformat();
					var gridrowData = griddata.GridRows.Where(x => x.TextId == input.TextId).FirstOrDefault();
					return Json(MapToGridModel(gridrowData));
				}

			}
			else
			{
				updateMode.Mode=input.ModeOfTranslation;
				ITranslationMode.UpdateTranslationMode(updateMode);

				if (updateMode.Mode == 2)
				{
					var updateGriddata = this.Gridformat();
					var updateGridrowData = updateGriddata.GridRows.Where(x => x.TextId == input.TextId).FirstOrDefault();
					return Json(MapToGridModel(updateGridrowData));
				}
			}
			
		
				var getModes= ITranslationMode.GetTranslationModeByID(input.TextId,input.LanguageCode);

				if(getModes.Mode==0 || getModes.Mode==1)
				{
					var repetTranslated = ITranslation.GetTranslation().Where(x => x.TextId == input.TextId && x.LanguageCode == input.LanguageCode && x.Translated_Text == input.TranslationText).FirstOrDefault();
					if (repetTranslated != null)
					{
						return Json(repetTranslated);
					}
					var TranslatedData = ITranslation.GetTranslation().Where(x => x.TextId == input.TextId && x.LanguageCode == input.LanguageCode && x.OfficialBoolean == true).FirstOrDefault();

					var Translatedtext = new Translation();
					Translatedtext.Translated_Text = input.TranslationText;
					Translatedtext.LanguageCode = input.LanguageCode;
					Translatedtext.TextId = input.TextId;
					if (TranslatedData != null)
					{
						if (TranslatedData.Votes > 0)
						{
							Translatedtext.Votes = 0;
							Translatedtext.OfficialBoolean = false;
						}
						else
						{
							TranslatedData.OfficialBoolean = false;
							ITranslation.UpdateTranslation(TranslatedData);
							Translatedtext.Votes = 0;
							Translatedtext.OfficialBoolean = true;

						}
					}
					else
					{
						Translatedtext.Votes = 0;
						Translatedtext.OfficialBoolean = true;

					}

					ITranslation.InsertTranslation(Translatedtext);

					TranslationLog translation_log = new TranslationLog();
					translation_log.TextId = Translatedtext.TextId;
					translation_log.System_Date = DateTime.Now;
					translation_log.LanguageCode = Translatedtext.LanguageCode;
					translation_log.Translated_Text = Translatedtext.Translated_Text;

					ITranslation_Log.InsertTranslationLog(translation_log);
					
				}
				var data = this.Gridformat();
					var rowData = data.GridRows.Where(x => x.TextId == input.TextId).FirstOrDefault();
					return Json(MapToGridModel(rowData));

			
		}

		public ActionResult Delete(string id, string gridId)
		{

			return PartialView(new DeleteConfirmInput
			{
				TextId = id,
				Message = string.Format("Are you sure you want to delete")
			});
		}

		[HttpPost]
		public ActionResult Delete(DeleteConfirmInput input)
		{
			ITranslationArchive.InsertDeletedRecords(input.TextId);
			ITranslation.DeleteTranslation(input.TextId);
			ITranslation_Log.DeleteTranslationLog(input.TextId);
			return Json(new { Id = input.TextId });
		}

	}
}
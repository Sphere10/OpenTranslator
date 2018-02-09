using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Net;
using System.Web.UI.WebControls;

using Omu.AwesomeMvc;

using OpenTranslator.Data;
using OpenTranslator.Utils;
using OpenTranslator.Models.Input;
using OpenTranslator.Repostitory;
using OpenTranslator.Models;

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

        #region Constructor
        
        public AdminController()
		{
			ITranslation = new TranslationRepository();
			ILanguages = new LanguageRepository();
			ITranslation_Log = new TranslationLogRepository();
			ITranslationArchive = new TranslationArchiveRepository();
			IVotes = new VotesRepository();
			ITranslationMode = new TranslationModeRepository();
		}

        #endregion


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
			list = ILanguages.GetAll().ToList();
			var column = new List<String>();
			for (int i = 0; i < list.Count; i++)
			{
				if (!list[i].LanguageCode.Equals("en"))
					column.Add(list[i].LanguageName.ToString());
			}
			var col = column.ToArray();
			columns = columns ?? col;
			object value = null;
			if (Session["SelectedColumns"] != null)
			{
				value = (string[])Session["SelectedColumns"];
			}

			//return Json(columns.Select(o => new KeyContent(o, o)));
			return Json(new AweItems
			{
				Items = columns.Select(o => new KeyContent(o, o)),
				Value = value
			});
		}

		public ActionResult Index()
		{
			if (Request.Cookies["UserId"] == null)
			{
				return RedirectToAction("Index", "User");
			}

			return View("Index","_AdminLayout");

		}

		public ActionResult embeded()
		{
			return View("Index","_LayoutEmbedAdmin");
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

		public DataTable getTable()
		{
				var languages = ILanguages.GetAll().Select(x => x.LanguageCode).Distinct();
			var query = from r in ITranslation.GetAll().Where(x => x.OfficialBoolean == true)
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
			var languagesList = ILanguages.GetAll().Select(x => x.LanguageName).Distinct();
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
			return table;
		}

		public GridFormatData Gridformat()
		{
			var table= getTable();
		
			if(System.Web.HttpContext.Current.Request.Cookies["MissingTrans"] != null)
			{
				DataTable dtNewtable = new DataTable();
				if (System.Web.HttpContext.Current.Session["SelectedColumns"] != null)
				{
					string[] selectedColumns = (string[])System.Web.HttpContext.Current.Session["SelectedColumns"];
					foreach (var column in selectedColumns)
					{
						for (int i = table.Rows.Count - 1; i >= 0; i--)
						{
							// whatever your criteria is

							if (table.Rows[i][column].ToString() != "" && System.Web.HttpContext.Current.Request.Cookies["MissingTrans"].Value == "true")
								table.Rows[i].Delete();
						}
					}
				}


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


		public ActionResult OriginalTextGridGetItems(GridParams g, string[] selectedColumns, bool? choosingColumns, string UserType = "Admin", bool clearsession = false)
		{
			if (selectedColumns != null)
			{
				Session["SelectedColumns"] = selectedColumns;

			}

			var GridData = this.Gridformat();
			var columns = GridData.GridColumn.ToArray();
			var deletecolomn = columns[columns.Length-1];
			
			if (clearsession)
				Session.Clear();
			if (Session["SelectedColumns"] != null)
			{
				selectedColumns = (string[])Session["SelectedColumns"];
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

			selectedColumns = selectedColumns ?? new string[] { };

			//make sure we always have Id and Person columns
			selectedColumns = selectedColumns.Union(baseColumns).ToArray();

			var currectColumns = g.Columns.ToList();

			//remove unselected columns
			currectColumns = currectColumns.Where(o => selectedColumns.Contains(o.Header)).ToList();

			//add missing columns
			var missingColumns = selectedColumns.Except(currectColumns.Select(o => o.Header)).ToArray();

			currectColumns.AddRange(columns.Where(o => missingColumns.Contains(o.Header)));

			if (UserType.Equals("Admin"))
				currectColumns.Add(deletecolomn);
			
			g.Columns = currectColumns.ToArray();


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
			if (!ModelState.IsValid ||ITranslation.IsTextIdAlreadyStoraged(input)==true || ITranslation.IsTextAlreadyStoraged(input, input.Text)==true)
			{
				ViewBag.errormsg = "TextId or Text already exist.";
				return PartialView(input);
			}
			
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

            if (System.Web.HttpContext.Current.Request.Cookies["MissingTrans"] != null)
			{
				if(System.Web.HttpContext.Current.Request.Cookies["MissingTrans"].Value != "true")
				{ 
					return Json(MapToGridModel(rowData));
				}

                return Json(rowData);
			}
			else
			{
				return Json(rowData);
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
		public ActionResult GetUserCurrentVote(int Id)
		{

			if (Request.Cookies["BrowserId"] != null)
			{
				Guid cookieValue = Guid.Parse(Request.Cookies["BrowserId"].Value);
				var vote = IVotes.GetVoteByTranslationID(Id, cookieValue);
                bool isValid = vote != null ? true : false;

                return Json(new { value = isValid });
				
			}
		
        	return Json(new { value = false });


		}

		[HttpPost]
		public ActionResult VoteCount(string vote, string textid, decimal TranslationId, string code)
		{
			var getMode = ITranslationMode.GetTranslationModeByID(textid, code);

			if (getMode.Mode == 2)
			{
				return PartialView();
			}
			else
			{
				if (System.Web.HttpContext.Current.Request.Cookies["BrowserId"] == null)
				{
					Guid id = Guid.NewGuid();
					System.Web.HttpContext.Current.Response.Cookies["BrowserId"].Value = id.ToString();
					System.Web.HttpContext.Current.Response.Cookies["BrowserId"].Expires = DateTime.Now.AddMonths(1);
				}

				Guid cookieValue = Guid.Parse(System.Web.HttpContext.Current.Request.Cookies["BrowserId"].Value);
				string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
				string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();


				//var session = HttpContext.Session.SessionID;
				var voteData = IVotes.GetVoteByCookieID(cookieValue);

				var items = ITranslation.GetTranslationLogByCode(textid, code).ToList();
				var list = items;
				var data = items.Where(x => x.Id == TranslationId).FirstOrDefault();
				if (voteData == null)
				{
					data.Votes = data.Votes + 1;
					Vote addVote = new Vote();
					addVote.CookieID = cookieValue;
					addVote.Translation_Id = data.Id;
					addVote.IP = myIP;
					IVotes.Save(addVote);

				}
				else
				{
					var voteList = IVotes.GetVoteList(cookieValue);
					var voteResult = voteList.Where(x => items.Any(p => p.Id == x.Translation_Id));
					foreach (var item in items)
					{
						var id = Convert.ToInt32(item.Id);
						var updateVote = IVotes.GetVoteByTranslationID(id, cookieValue);
						//
						if (updateVote != null)
						{
							item.Votes = item.Votes == 0 ? 0 : item.Votes - 1;
							data.Votes = data.Votes + 1;
							ITranslation.Update(item);
							IVotes.Delete(int.Parse(updateVote.Id.ToString()));
							Vote addVote = new Vote();
							addVote.CookieID = cookieValue;
							addVote.Translation_Id = data.Id;
							addVote.IP = myIP;
							IVotes.Save(addVote);


						}

					}
					if (voteResult.Count() == 0)
					{
						data.Votes = data.Votes + 1;
						Vote addVote = new Vote();
						addVote.CookieID = cookieValue;
						addVote.Translation_Id = data.Id;
						addVote.IP = myIP;
						IVotes.Save(addVote);
					}
				}
				if (getMode.Mode == 1)
				{
					ITranslation.Update(data);
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

						ITranslation.Update(setdata);

					}
					ITranslation.Update(data);

					return Json(data);
				}

			}
		}


		public ActionResult Edit(string TextId, string code, string UserType = "Admin")
		{
			var viewname = "";
			if (UserType.Equals("Admin"))
			{
				viewname = "EditTranslation";
			}
			else
			{
				viewname = "../User/EditTranslation";
			}
			Language languages = ILanguages.GetAll().Where(x => x.LanguageName == code).FirstOrDefault();
			Translation translation = ITranslation.GetAll().Where(x => x.TextId == TextId && x.LanguageCode == languages.LanguageCode && x.OfficialBoolean == true).FirstOrDefault();
			var translationMode = ITranslationMode.GetTranslationModeByID(TextId, languages.LanguageCode);

			if (translation == null)
			{
				return PartialView(viewname, new TranslationInput { TextId = TextId, TranslationText = null, LanguageCode = languages.LanguageCode, ModeOfTranslation = 0 });
			}
			else
			{
				var id = Convert.ToInt32(translation.Id.ToString());
				if (translationMode == null)
				{
					return PartialView(viewname, new TranslationInput { TextId = TextId, TranslationText = translation.Translated_Text, LanguageCode = languages.LanguageCode, Id = id, ModeOfTranslation = 0 });
				}
				else
				{
					if (translationMode.Mode == 2 && viewname == "../User/EditTranslation")
					{
						ViewBag.errormsg = "Translation Mode is Locked you can not add vote or edit this translation.";
					}
					return PartialView(viewname, new TranslationInput { TextId = TextId, TranslationText = translation.Translated_Text, LanguageCode = languages.LanguageCode, Id = id, ModeOfTranslation = translationMode.Mode });
				}
			}

		}

		[HttpPost]
		public ActionResult Edit(TranslationInput input)
		{
			//TODO: still need some improvements
			if (!ModelState.IsValid)
			{
				return PartialView("EditTranslation", input);
			}
			input.Votes = 1;

			var updateMode = ITranslationMode.GetTranslationModeByID(input.TextId, input.LanguageCode);

            if (updateMode == null)//This mean it is the first Translation of this string
			{
				TranslationMode mode = new TranslationMode();
				mode.TextId = input.TextId;
				mode.LanguageCode = input.LanguageCode;
				mode.Mode = input.ModeOfTranslation;
				ITranslationMode.InsertTranslationMode(mode);

                updateMode = mode;

				if (input.ModeOfTranslation == 2)
				{
					var griddata = this.Gridformat();
					var gridrowData = griddata.GridRows.Where(x => x.TextId == input.TextId).FirstOrDefault();
					return Json(MapToGridModel(gridrowData));
				}

			}
			else
			{
				updateMode.Mode = input.ModeOfTranslation;
				ITranslationMode.UpdateTranslationMode(updateMode);

				if (updateMode.Mode == 2)
				{
					var updateGriddata = this.Gridformat();
					var updateGridrowData = updateGriddata.GridRows.Where(x => x.TextId == input.TextId).FirstOrDefault();
					return Json(MapToGridModel(updateGridrowData));
				}
			}

			var newtranslation = new Translation();
			if (updateMode.Mode == 0 || updateMode.Mode == 1)
			{
				var repetTranslated = ITranslation.GetAll().Where(x => x.TextId == input.TextId && x.LanguageCode == input.LanguageCode && x.Translated_Text.ToLower() == input.TranslationText.ToLower()).FirstOrDefault();
				if (repetTranslated != null)
				{
					if (updateMode.Mode == 0)
					{
						return Json(repetTranslated);
					}
					
				}

                //Find the las translation that has officialBoolean in true
				var translatedData = ITranslation.GetAll()
                    .Where( x => 
                        x.TextId == input.TextId && 
                        x.LanguageCode == input.LanguageCode && 
                        x.OfficialBoolean == true)
                    .FirstOrDefault();

                if (translatedData != null)
                {
                    translatedData.OfficialBoolean = false;
                    translatedData.Votes = translatedData.Votes;

                    //Update the last saved translation with the new values in order to put the new translation as the one to be shown in the grid.
                    // translateData is NOT the new input to be saved, is the last saved in a previous instance
                    ITranslation.Update(translatedData);
                }

                // the new translation to be saved in db
                var translatedText = new Translation();
				translatedText.Translated_Text = input.TranslationText;
				translatedText.LanguageCode = input.LanguageCode;
				translatedText.TextId = input.TextId;
                translatedText.Votes = 0;
                translatedText.OfficialBoolean = true;

				ITranslation.Save(translatedText);

				newtranslation = ITranslation.GetAll().Where(x => x.Id == translatedText.Id).FirstOrDefault();

                this.VoteCount("Like", newtranslation.TextId, newtranslation.Id, newtranslation.LanguageCode);

				TranslationLog translation_log = new TranslationLog();
				translation_log.TextId = translatedText.TextId;
				translation_log.System_Date = DateTime.Now;
				translation_log.LanguageCode = translatedText.LanguageCode;
				translation_log.Translated_Text = translatedText.Translated_Text;

				ITranslation_Log.Save(translation_log);
				
			}

			var data = this.Gridformat();
			var rowData = data.GridRows.Where(x => x.TextId == input.TextId).FirstOrDefault();

			if (System.Web.HttpContext.Current.Request.Cookies["MissingTrans"] != null)
			{
				if (System.Web.HttpContext.Current.Request.Cookies["MissingTrans"].Value != "true")
				{
					return Json(MapToGridModel(rowData));
				}
				else
				{
					return Json(data);
				}
			}
			else
			{
				return Json(data);
			}



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
			var items = ITranslation.GetTranslationByTextID(input.TextId).ToList();
			ITranslationArchive.InsertDeletedRecords(input.TextId);

            //Delete the corresponding range in db
			ITranslation.DeleteRange(input.TextId);

			foreach (var item in items)
			{
				var vote = IVotes.GetVoteByTranslationID(item.Id);
				if (vote != null)
				{
					IVotes.Delete(int.Parse(vote.Id.ToString()));
				}
			}
			ITranslation_Log.DeleteRange(input.TextId);
			return Json(new { Id = input.TextId });
		}

	}
}

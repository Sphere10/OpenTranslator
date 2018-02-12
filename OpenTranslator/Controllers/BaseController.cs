using OpenTranslator.Repostitory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenTranslator.Controllers
{
    public class BaseController : Controller
    {
        #region private properties

        static readonly BaseController instance = new BaseController();

        private IUsers iUserRepository = new UsersRepository();
        private ILanguages iLanguagesRepository = new LanguageRepository();
        private ITranslationLog iTranslationLogRepository = new TranslationLogRepository();
        private ITranslationArchive iTranslationArchiveRepository = new TranslationArchiveRepository();
        private IVotes iVotesRepository = new VotesRepository();
        private ITranslationMode iTranslationModeRepository = new TranslationModeRepository();
        private ITranslation iTranslationRepository = new TranslationRepository();

        #endregion

        #region public methods and properties

        /// <summary>
		///     Singleton instance for default Base Controller
		/// </summary>
		public static BaseController Instance => instance;


        public IUsers IUser => iUserRepository;

        public ILanguages ILanguages => iLanguagesRepository;

        public ITranslationLog ITranslationLog => iTranslationLogRepository;

        public ITranslationArchive ITranslationArchive => iTranslationArchiveRepository;

        public IVotes IVotes => iVotesRepository;

        public ITranslationMode ITranslationMode => iTranslationModeRepository;

        public ITranslation ITranslation => iTranslationRepository;

        #endregion
    }
}
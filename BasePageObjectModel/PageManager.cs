﻿using OpenQA.Selenium;
using System;
using System.Linq;
using OpenQA.Selenium.Chrome;

namespace BasePageObjectModel
{
	public class PageManager : IDisposable
	{
		private static PageManager _current;

		public PageManager(string baseUrl)
		{
			BaseUrl = new Uri(baseUrl);
		}

		public virtual void Initialize()
		{
			if (WebDriver == null)
			{
				WebDriver = new ChromeDriver();
			}
		}

		public virtual void Dispose()
		{
			if (WebDriver != null)
			{
				WebDriver.Close();
				WebDriver.Quit();
			}
		}

		public static PageManager Current
		{
			get { return _current; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}
				_current = value;
			}
		}

		public T GetMatchingPage<T>() where T : BaseBasePage
		{
			return BasePages.FirstOrDefault(p => p.GetType() == typeof(T)) as T;
		}

		public virtual BaseBasePage CurrentPage
		{
			get
			{
				return BasePages.FirstOrDefault(page => page.IsUrlDisplayed());
			}
		}

		protected BaseBasePage[] GetPagesInAssembly(PageManager pageManager)
		{
			var pages = from t in GetType().Assembly.GetTypes()
						where t.IsSubclassOf(typeof(BaseBasePage))
							  && !t.IsAbstract
						select (BaseBasePage)Activator.CreateInstance(t, WebDriver);
			return pages.ToArray();
		}
		
		private readonly Lazy<BaseBasePage[]> basePages = new Lazy<BaseBasePage[]>(() => Current.GetPagesInAssembly(Current));
		public BaseBasePage[] BasePages => basePages.Value;

		public IWebDriver WebDriver { get; set; }
		public Uri BaseUrl { get; set; }
	}
}

﻿using ApiExplorer.Utilities;
using Microsoft.Practices.Composite.Presentation.Commands;
using Ramone;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ApiExplorer.ViewModels
{
  public class ComposerViewModel : ViewModel
  {
    public class MethodDefinition
    {
      public string Name { get; set; }
      public bool AllowContent { get; set; }
    }

    
    #region UI properties

    private string _windowTitle;
    public string WindowTitle
    {
      get { return _windowTitle; }
      set
      {
        if (value != _windowTitle)
        {
          _windowTitle = value;
          OnPropertyChanged("WindowTitle");
        }
      }
    }


    public ObservableCollection<MethodDefinition> Methods { get; set; }

    private string _method;
    public string Method
    {
      get { return _method; }
      set
      {
        if (value != _method)
        {
          _method = value;
          OnPropertyChanged("Method");
          OnPropertyChanged("MethodAllowsContent");
        }
      }
    }


    public bool MethodAllowsContent
    {
      get
      {
        MethodDefinition def = Methods.Where(m => m.Name == Method).FirstOrDefault();
        return def == null || def.AllowContent;
      }
    }


    private string _url;
    public string Url
    {
      get { return _url; }
      set
      {
        if (value != _url)
        {
          _url = value;
          OnPropertyChanged("Url");
        }
      }
    }


    private string _contentType;
    public string ContentType
    {
      get { return _contentType; }
      set
      {
        if (value != _contentType)
        {
          _contentType = value;
          OnPropertyChanged("ContentType");
        }
      }
    }


    private static string _headers;
    public string Headers
    {
      get { return _headers; }
      set
      {
        if (value != _headers)
        {
          _headers = value;
          OnPropertyChanged("Headers");
        }
      }
    }


    private string _body;
    public string Body
    {
      get { return _body; }
      set
      {
        if (value != _body)
        {
          _body = value;
          OnPropertyChanged("Body");
        }
      }
    }


    #endregion


    #region Commands

    public DelegateCommand<FrameworkElement> ExecuteCommand { get; private set; }

    #endregion


    public ComposerViewModel(ViewModel parent)
      : base(parent)
    {
      RegisterCommand(ExecuteCommand = new DelegateCommand<FrameworkElement>(Execute));
      Url = "";
      Methods = new ObservableCollection<MethodDefinition>
      {
        new MethodDefinition { Name = "GET", AllowContent = false },
        new MethodDefinition { Name = "POST", AllowContent = true },
        new MethodDefinition { Name = "PUT", AllowContent = true },
        new MethodDefinition { Name = "DELETE", AllowContent = false },
        new MethodDefinition { Name = "PATCH", AllowContent = true }
      };
    }


    private void Execute(FrameworkElement sender)
    {
      try
      {
        Uri url = new Uri(Url);
      }
      catch (Exception)
      {
        MessageBox.Show("Invalid URL");
        return;
      }

      if (string.IsNullOrEmpty(Method))
      {
        MessageBox.Show("Missing HTTP method");
        return;
      }

      ISession session = RamoneServiceManager.Service.NewSession();

      Request req = session.Bind(Url).Method(Method);

      if (Headers != null)
      {
        foreach (string line in Headers.Split('\n'))
        {
          int colonPos = line.IndexOf(':');
          if (colonPos > 0)
          {
            string header = line.Substring(0, colonPos).Trim();
            string value = line.Substring(colonPos + 1).Trim();
            if (!string.IsNullOrEmpty(header) && !string.IsNullOrEmpty(value))
            {
              if (header.Equals("accept", StringComparison.InvariantCultureIgnoreCase))
                req.Accept(value);
              else if (!header.Equals("content-type", StringComparison.InvariantCultureIgnoreCase))
                req.Header(header, value);
            }
          }
        }
      }

      if (ContentType != null)
        req.ContentType(ContentType);

      if (Body != null)
        req.Body(Body);

      Window w = Window.GetWindow(sender as DependencyObject);

      Publish(new ExecuteWebRequestEventArgs { Request = req, OnSuccess = (r => HandleSuccess(r, w)) });
    }


    private void HandleSuccess(Response r, Window w)
    {
      w.Close();
    }
  }
}
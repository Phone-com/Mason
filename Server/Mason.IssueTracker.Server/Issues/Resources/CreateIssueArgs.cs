﻿namespace Mason.IssueTracker.Server.Issues.Resources
{
  public class CreateIssueArgs
  {
    public string Title { get; set; }

    public string Description { get; set; }

    public int Severity { get; set; }
  }
}

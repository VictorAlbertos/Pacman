namespace UnityEngine.Advertisements {
  using System;

  public class ShowOptions {

    public bool pause { get; set; }

    public Action<ShowResult> resultCallback { get; set; }

  }

}

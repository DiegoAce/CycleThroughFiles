using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using CycleThroughFiles.Helpers;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;
using System.Linq;
using System.IO;
using Microsoft;

namespace CycleThroughFiles.Commands
{
  /// <summary>
  /// Command handler
  /// </summary>
  internal sealed class CycleThroughFilesCommand 
  {
    /// <summary>
    /// Command ID.
    /// </summary>
    public const int CommandId = 0x0100;

    /// <summary>
    /// Command menu group (command set GUID).
    /// </summary>
    public static readonly Guid CommandSet = new Guid("0495e283-a839-40d4-8baa-89ef232f5738");

    /// <summary>
    /// VS Package that provides this command, not null.
    /// </summary>
    private readonly AsyncPackage _package;

    private readonly DTE2 _dte;

    private IServiceProvider ServiceProvider => _package;

    /// <summary>
    /// Initializes a new instance of the <see cref="CycleThroughFilesCommand"/> class.
    /// Adds our command handlers for menu (commands must exist in the command table file)
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    /// <param name="commandService">Command service to add command to, not null.</param>
    private  CycleThroughFilesCommand(AsyncPackage package)
    {
      _package = package ?? throw new ArgumentNullException(nameof(package));
     
      _dte = ServiceProvider.GetService(typeof(DTE)) as DTE2;
      Assumes.Present(_dte);


      if (ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
      {
        var menuCommandID = new CommandID(CommandSet, CommandId);
        var menuItem = new OleMenuCommand(this.Execute, this.QueryChange, this.QueryStatus, menuCommandID);

        commandService.AddCommand(menuItem);
      }



    
    }

    /// <summary>
    /// Gets the instance of the command.
    /// </summary>
    public static CycleThroughFilesCommand Instance
    {
      get;
      private set;
    }


    private void QueryChange(object sender, EventArgs e)
    {
      var button = (OleMenuCommand)sender;
      button.Enabled = _dte.ActiveDocument != null;

    }


    private void QueryStatus(object sender, EventArgs e)
    {
      var button = (OleMenuCommand)sender;
      button.Enabled = _dte.ActiveDocument != null;

    }

    /// <summary>
    /// Initializes the singleton instance of the command.
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    public static async Task InitializeAsync(AsyncPackage package)
    {
      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
      Instance =  new CycleThroughFilesCommand(package);
      

    }

    

  


    /// <summary>
    /// This function is the callback used to execute the command when the menu item is clicked.
    /// See the constructor to see how the menu item is associated with this function using
    /// OleMenuCommandService service and MenuCommand class.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event args.</param>
    private void Execute(object sender, EventArgs e)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      if (_dte.ActiveDocument is null) return;

      // get the list of actual files extensions that matches current file name without extension :

      var curntFile = Path.GetFileName( _dte.ActiveDocument.FullName);
      var curntPath = Path.GetDirectoryName(_dte.ActiveDocument.FullName);

      var siblings = DocumentHelpers.Siblings(_dte.ActiveDocument.FullName).ToArray();
     
      if (siblings.Count() <= 1) return;
            

      VsShellUtilities.OpenDocument(ServiceProvider, Path.Combine ( curntPath,  DocumentHelpers.NextSibling( siblings, curntFile)));

      


    }
  }
}

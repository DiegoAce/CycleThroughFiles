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
  
  internal sealed class CycleThroughFilesReverseCommand
  {
    /// <summary>
    /// Command ID.
    /// </summary>
    public const int CommandId = 0x0101;

    /// <summary>
    /// Command menu group (command set GUID).
    /// </summary>
    public static readonly Guid CommandSet = new Guid("0495e283-a839-40d4-8baa-89ef232f5738");

    /// <summary>
    /// VS Package that provides this command, not null.
    /// </summary>
    private readonly Package _package;

    private readonly DTE2 _dte;

    private IServiceProvider ServiceProvider => _package;

    /// <summary>
    /// Initializes a new instance of the <see cref="CycleThroughFilesCommand"/> class.
    /// Adds our command handlers for menu (commands must exist in the command table file)
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    /// <param name="commandService">Command service to add command to, not null.</param>
    private CycleThroughFilesReverseCommand(Package package)
    {
      _package = package ?? throw new ArgumentNullException(nameof(package));
     
      _dte = ServiceProvider.GetService(typeof(DTE)) as DTE2;
      Assumes.Present(_dte);


      if (ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
      {
        var menuCommandID = new CommandID(CommandSet, CommandId);
        var menuItem = new OleMenuCommand(this.Execute,menuCommandID) { Supported = true };
        menuItem.BeforeQueryStatus += QueryStatus;

        commandService.AddCommand(menuItem);
      }



    
    }

    private void QueryStatus(object sender, EventArgs e)
    {
      var button = (OleMenuCommand)sender;
      button.Enabled = _dte.ActiveDocument != null;
     
    }


    /// <summary>
    /// Gets the instance of the command.
    /// </summary>
    public static CycleThroughFilesReverseCommand Instance
    {
      get;
      private set;
    }



    /// <summary>
    /// Initializes the singleton instance of the command.
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    /// <summary>
    /// Initializes the singleton instance of the command.
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    public static async Task InitializeAsync(AsyncPackage package)
    {
      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
      Instance = new CycleThroughFilesReverseCommand(package);


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


      VsShellUtilities.OpenDocument(ServiceProvider, Path.Combine ( curntPath,  DocumentHelpers.PreviousSibling( siblings, curntFile)));

      


    }
  }
}

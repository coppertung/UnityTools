using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Patterns {

	#region Interfaces
	/// <summary>
	/// Interface used for command pattern.
	/// </summary>
	public interface ICommand {

		/// <summary>
		/// Execute the command.
		/// </summary>
		void execute();
		/// <summary>
		/// Undo the command.
		/// </summary>
		void undo();

	}
	#endregion

	#region Data_Classes
	/// <summary>
	/// Command element, which stores command id and the command class.
	/// </summary>
	public class CommandElement {

		/// <summary>
		/// The identifier of the command.
		/// </summary>
		public int id;
		/// <summary>
		/// Command class.
		/// </summary>
		public ICommand command;

	}
	#endregion

	/// <summary>
	/// Command controller, which control the order and execution of the commands here.
	/// All commands should be inherited from the interface ICommand.
	/// ** REQUIRE TEST **
	/// Reference from:
	/// http://www.habrador.com/tutorials/programming-patterns/1-command-pattern/
	/// http://gameprogrammingpatterns.com/command.html
	/// </summary>
	public class CommandController {

		#region Fields_And_Properties
		private int commandCount;

		/// <summary>
		/// The id of the last executed command.
		/// No command has been applied or command list has been reseted if it returns -1.
		/// </summary>
		public int currentCommandID {
			get;
			protected set;
		}
		/// <summary>
		/// The command list, which stores the full history of the commands.
		/// </summary>
		public List<CommandElement> commandList {
			get;
			protected set;
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor, which do initialization here.
		/// </summary>
		public CommandController() {

			currentCommandID = -1;
			commandList = new List<CommandElement> ();

		}
		#endregion

		#region Functions
		/// <summary>
		/// Add a new command.
		/// </summary>
		public void newCommand(ICommand command) {

			CommandElement newCommandElement = new CommandElement ();
			newCommandElement.id = commandCount;
			newCommandElement.command = command;
			commandList.Add (newCommandElement);
			commandCount += 1;

		}

		/// <summary>
		/// Remove the specified command.
		/// </summary>
		public void removeCommand (ICommand command) {

			for (int i = 0; i < commandList.Count; i++) {
				if (commandList [i].command == command) {
					commandList.RemoveAt (i);
					break;
				}
			}

		}

		/// <summary>
		/// Remove the command with specified id.
		/// </summary>
		public void removeCommandAt(int commandID) {

			commandList.RemoveAt (commandID);

		}

		/// <summary>
		/// Remove the commands before specified id.
		/// </summary>
		public void removeCommandBefore(int commandID) {

			for (int i = commandList.Count; i > 0; i--) {
				if (commandList [i].id < commandID) {
					commandList.RemoveAt (i);
				}
			}

		}

		/// <summary>
		/// Remove the commands after specified id.
		/// </summary>
		public void removeCommandAfter(int commandID) {

			for (int i = commandList.Count; i > 0; i--) {
				if (commandList [i].id > commandID) {
					commandList.RemoveAt (i);
				}
			}
		}

		/// <summary>
		/// Clear all the commands.
		/// </summary>
		public void clearCommand() {

			currentCommandID = -1;
			commandList.Clear ();

		}

		/// <summary>
		/// Reset the command controller.
		/// </summary>
		public void reset() {

			commandCount = 0;
			clearCommand ();

		}

		/// <summary>
		/// Execute the command with specified id.
		/// </summary>
		public void executeCommand(int commandID) {

			currentCommandID = commandID;
			commandList [commandID].command.execute ();

		}

		/// <summary>
		/// Undo the commands until the specified id.
		/// </summary>
		public void undoCommandsTill(int commandID) {

			for (int i = commandList.Count; i > 0; i--) {
				if (commandList [i].id > commandID) {
					commandList [i].command.undo ();
				} else {
					currentCommandID = commandList [i].id;
					break;
				}
			}

		}
		#endregion

	}

}
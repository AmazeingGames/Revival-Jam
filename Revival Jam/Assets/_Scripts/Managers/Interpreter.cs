using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static DialogueBank;

public class Interpreter : Singleton<Interpreter>
{
    List<string> response = new();

    readonly HelpCommand help = new("help");
    readonly NoteCommand wrenchNote = new(DialogueType.Wrench, "note1", "wrench", "jump");
    readonly NoteCommand powerNote = new(DialogueType.Power, "note2", "power");
    readonly NoteCommand wiresNote = new(DialogueType.Wires, "note3", "crowbar", "wires", "controls");
    readonly NoteCommand shakeNote = new(DialogueType.Shake, "note4", "shake");
    readonly NoteCommand hammerNote = new(DialogueType.Hammer, "note5", "hammer", "break");
    readonly ClearCommand clearCommand = new("clear", "clear console");

    readonly UnknownCommand unknown = new();

    List<Command> commands = new();

    private void Start()
    {
        commands = new List<Command>()
        {
            help,
            wrenchNote,
            powerNote,
            wiresNote,
            shakeNote,
            hammerNote,
            clearCommand
        };
    }

    //Finds and executes the matching command
    public Command Interpret(string userInput)
    {
        response.Clear();

        userInput = userInput.Replace(" ", "");

        for (int i = 0; i < commands.Count; i++)
        {
            var command = commands[i];

            if (command.DoesCallMatch(userInput))
                return command;
        }
        return unknown;
    }

    class UnknownCommand : Command
    {
        public override void Execute()
        {
            TerminalManager.Instance.ReadyInput();
        }
    }

    //Disables all instantiated lines in the terminal screen
    //To Do: Return all these objects to an object pool
    class ClearCommand : Command
    {
        public ClearCommand(params string[] commandKeys) : base  (commandKeys) { }

        public override void Execute()
        {
            List<GameObject> linesToClear = TerminalManager.Instance.ResponseLines
                                        .Select(l => l.gameObject)
                                        .ToList();

            linesToClear.AddRange(TerminalManager.Instance.MimicLines);

            foreach (var line in linesToClear)
                line.SetActive(false);

            TerminalManager.Instance.SetCommandLineSize(TerminalManager.CommandLineSet.Reset);
            TerminalManager.Instance.ReadyInput();
        }

    }

    class HelpCommand : Command
    {
        public HelpCommand(params string[] commandKeys) : base (commandKeys) { }

        override public void Execute()
        {
            TerminalManager.Instance.ReadyInput();
        }
    }

    class NoteCommand : Command
    {
        readonly DialogueType dialogueToPlay;
        public NoteCommand(DialogueType dialogueToPlay, params string[] commandKeys) : base(commandKeys) 
        {
            
            this.dialogueToPlay = dialogueToPlay;
        }

        override public void Execute() 
        {
            TerminalManager.Instance.DisableInput();
            TerminalManager.Instance.ScrollToBottom();
            DialogueManager.Instance.StartDialogue(dialogueToPlay);
        }
    }

    public abstract class Command
    {
        //public IList<string> CommandKeys { get => commandKeys.AsReadOnlyList(); }
        readonly List<string> commandCall;

        public Command(params string[] commandKeys) 
        {
            this.commandCall = commandKeys.ToList();    
        }

        public abstract void Execute();

        public bool DoesCallMatch(string key)
        {
            string keyLower = key.ToLower();

            for (int i = 0; i < commandCall.Count; i++)
            {
                var currentLower = commandCall[i].ToLower();

                if (currentLower == keyLower)
                    return true;
            }
            return false;
        }
    }
}

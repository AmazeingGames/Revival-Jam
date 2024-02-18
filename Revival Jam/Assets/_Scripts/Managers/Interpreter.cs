using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using static DialogueBank;
using static TerminalManager;

public class Interpreter : Singleton<Interpreter>
{
    readonly List<string> response = new();

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

    
    private void OnGUI()
    {
        var terminalInput = TerminalManager.Instance.TerminalInput;

        //Instead of checking for empty text, we could instead put a cap that this can be called in one second
        if (terminalInput.isFocused && terminalInput.text != "" && Input.GetKeyDown(KeyCode.Return))
        {
            string userInput = terminalInput.text;
            terminalInput.text = "";

            TerminalManager.Instance.AdjustCommandLineSize(CommandLineSet.Add, 30);
            TerminalManager.Instance.MimicInput(userInput);

            //Run the user's found command
            Interpret(userInput).Execute();

            Debug.Log($"Adjusted size and ran interpret command. User Input = \"{userInput}\"");

        }
    }
    

    class UnknownCommand : Command
    {
        public override void Execute()
        {
            TerminalManager.Instance.ReadyInput();
        }
    }

    //Returns all instantiated lines in the terminal screen to the object pool
    class ClearCommand : Command
    {
        public ClearCommand(params string[] commandKeys) : base  (commandKeys: commandKeys) { }

        public override void Execute()
        {
            List<GameObject> linesToClear = TerminalManager.Instance.ResponseLines
                                        .Select(l => l.gameObject)
                                        .ToList();

            linesToClear.AddRange(TerminalManager.Instance.MimicLines);

            foreach (var line in linesToClear)
                line.SetActive(false);

            TerminalManager.Instance.AdjustCommandLineSize(TerminalManager.CommandLineSet.Reset);
            TerminalManager.Instance.ReadyInput();
        }
    }

    class HelpCommand : WriteCommand
    {
        public HelpCommand(params string[] commandKeys) : base("Press | TAB | to switch between views", commandKeys) { }
    }

    class CommandCommand : WriteCommand
    {
        public CommandCommand(params string[] commandKeys) : base("Write all commands", commandKeys)
        {

        }
    }

    class NoteCommand : WriteCommand
    {
        readonly DialogueType dialogueToPlay;
        public NoteCommand(DialogueType dialogueToPlay, params string[] commandKeys) : base(commandKeys: commandKeys) 
        {
            this.dialogueToPlay = dialogueToPlay;
        }

        protected override void CreateDialogue() =>
            dialogue = DialogueBank.Instance.DialogueDataByType[dialogueToPlay];
    }

    //Starting dialogue always readies input once dialogue finishes
    public abstract class WriteCommand : Command
    {
        readonly string dialogueText = "";
        protected Dialogue dialogue;

        public WriteCommand(string response = "", params string[] commandKeys) : base(commandKeys: commandKeys)
        {
            dialogueText = response;
        }

        protected virtual void CreateDialogue()
        {
            dialogue = Dialogue.CreateDialogue(dialogueText);
            Debug.Log("Created dialogue");
        }

        public override void Execute()
        {
            if (dialogue == null)
                CreateDialogue();

            TerminalManager.Instance.DisableInput();
            TerminalManager.Instance.ScrollToBottom();
            DialogueManager.Instance.StartDialogue(dialogue);
        }
    }

    //This should always ready dialogue as part of the execute method
    public abstract class Command
    {
        readonly List<string> commandCall;

        public Command(params string[] commandKeys) 
        {
            this.commandCall = commandKeys.ToList();
        }

        public abstract void Execute();

        //Determines if the given text 'unlocks' the command
        //'clear' -> clears the terminal
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

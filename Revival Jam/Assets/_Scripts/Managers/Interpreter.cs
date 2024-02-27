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
    [SerializeField] TerminalManager terminalManager;

    readonly List<string> response = new();

    readonly Help help = new("help");
    readonly NoteCommand wrenchNote = new(DialogueType.Wrench, "note1", "wrench", "jump");
    readonly NoteCommand powerNote = new(DialogueType.Power, "note2", "power");
    readonly NoteCommand wiresNote = new(DialogueType.Wires, "note3", "crowbar", "wires", "controls");
    readonly NoteCommand shakeNote = new(DialogueType.Shake, "note4", "shake");
    readonly NoteCommand hammerNote = new(DialogueType.Hammer, "note5", "hammer", "break");
    readonly Clear clearCommand = new("clear", "clear console");

    readonly Default unknown = new();

    List<Command> commands = new();

    private void Start()
    {
        Command.Init(terminalManager);

        commands = new List<Command>()
        {
            help,
            wrenchNote,
            powerNote,
            wiresNote,
            shakeNote,
            hammerNote,
            clearCommand,
            unknown,
        };
    }

    //Looks through the commands list to see if there's one matching the user input
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
        //Command not recognized
        return unknown;
    }

    private void OnGUI()
    {
        var terminalInput = terminalManager.TerminalInput;

        //Instead of checking for empty text, we could instead put a cap that this can be called in one second
        if (terminalInput.isFocused && terminalInput.text != "" && Input.GetKeyDown(KeyCode.Return))
        {
            string userInput = terminalInput.text;
            terminalInput.text = "";

            terminalManager.AdjustCommandLineSize(CommandLineSet.Add, 30);
            terminalManager.MimicInput(userInput);

            //Run the user's found command
            Interpret(userInput).Execute();
        }
    }

    class Default : WriteCommand
    {
        public Default(params string[] commandKeys) : base("Command not recognized", commandKeys) { }
    }

    //TO DO: Return cleared lines to the object pool
    class Clear : Command
    {
        public Clear(params string[] commandKeys) : base  (commandCalls: commandKeys) { }

        public override void Execute()
        {
            List<GameObject> linesToClear = TerminalManager.ResponseLines
                                        .Select(l => l.gameObject)
                                        .ToList();

            linesToClear.AddRange(TerminalManager.MimicLines);

            foreach (var line in linesToClear)
                line.SetActive(false);

            TerminalManager.AdjustCommandLineSize(CommandLineSet.Reset);
            TerminalManager.ReadyInput();
        }
    }

    //TO DO: Write out all commands
    class Help : WriteCommand
    {
        public Help(params string[] commandKeys) : base("Press | TAB | to switch between views", commandKeys) { }
    }

    class NoteCommand : WriteCommand
    {
        readonly DialogueType dialogueType;
        public NoteCommand(DialogueType dialogueToPlayType, params string[] commandKeys) : base(commandKeys: commandKeys) 
            => this.dialogueType = dialogueToPlayType;

        protected override void CreateDialogue() =>
            dialogue = GetDialogue(dialogueType);
    }

    public abstract class WriteCommand : Command
    {
        readonly string dialogueText = "";
        protected Dialogue dialogue;

        public WriteCommand(string response = "", params string[] commandKeys) : base(commandCalls: commandKeys)
            => dialogueText = response;

        protected virtual void CreateDialogue()
            => dialogue = Dialogue.CreateDialogue(dialogueText);

        public static Dialogue GetDialogue(DialogueType dialogueToPlay) 
            => DialogueBank.Instance.DialogueDataByType[dialogueToPlay];

        public override void Execute()
        {
            if (dialogue == null)
                CreateDialogue();

            DialogueManager.Instance.StartDialogue(dialogue);
        }
    }

    //Always ready dialogue as part of the execute method
    public abstract class Command
    {
        readonly List<string> commandCalls;
        protected TerminalManager TerminalManager { get => terminalManager; }
        static TerminalManager terminalManager;

        public Command(params string[] commandCalls) 
            => this.commandCalls = commandCalls.ToList();

        static public void Init(TerminalManager _terminalManager)
            => terminalManager = _terminalManager;

        public abstract void Execute();

        //Returns true if text matches the command call
        public bool DoesCallMatch(string key)
        {
            string keyLower = key.ToLower();

            for (int i = 0; i < commandCalls.Count; i++)
            {
                var currentLower = commandCalls[i].ToLower();

                if (currentLower == keyLower)
                    return true;
            }
            return false;
        }
    }
}

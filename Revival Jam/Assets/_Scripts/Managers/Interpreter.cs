using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Interpreter : MonoBehaviour
{
    List<string> response = new();

    readonly HelpCommand help = new("help");
    readonly NoteCommand note = new("note");
    readonly UnknownCommand unknown = new();

    List<Command> commands = new();

    private void Start()
    {
        commands = new List<Command>()
        {
            help,
            note,
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
            throw new System.NotImplementedException();
        }

        public override List<string> GetResponse()
        {
            var response = new List<string>
            {
                "I have no clue what you are saying, like literally no clue.",
                "Get some help. Seriously: 'help'."
            };

            return response;
        }
    }

    class HelpCommand : Command
    {
        public HelpCommand(params string[] commandKeys) : base (commandKeys) { }

        override public void Execute()
        {
            throw new System.NotImplementedException();
        }

        public override List<string> GetResponse()
        {
            var response = new List<string>
            {
                "If you need help, you should see a doctor.",
                "Alternatively, a therapist can work wonders."
            };

            return response;
        }
    }

    class NoteCommand : Command
    {
        public NoteCommand(params string[] commandKeys) : base(commandKeys) { }

        override public void Execute() 
        {
            throw new System.NotImplementedException(); 
        }

        public override List<string> GetResponse()
        {
            var response = new List<string>
            {
                "This is the first line of the note.",
                "This is the second line of the note."
            };

            return response;
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

        public abstract List<string> GetResponse();

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

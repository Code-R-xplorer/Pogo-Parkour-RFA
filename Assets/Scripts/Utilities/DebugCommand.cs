using System;

namespace Utilities
{
    // Base class for debug commands
    public class DebugCommandBase
    {
        public string CommandId { get; } // Unique identifier for the command
        public string CommandDescription { get; } // Description of what the command does
        public string CommandFormat { get; } // Format of how to use the command

        // Constructor for the DebugCommandBase class
        protected DebugCommandBase(string id, string description, string format)
        {
            CommandId = id;
            CommandDescription = description;
            CommandFormat = format;
        }
    }

    // Debug command without parameters
    public class DebugCommand : DebugCommandBase
    {
        private readonly Action command; // Action to be executed when this command is invoked
    
        // Constructor for the DebugCommand class
        public DebugCommand(string id, string description, string format, Action command) 
            : base(id, description, format)
        {
            this.command = command;
        }

        // Method to invoke the command's action
        public void Invoke()
        {
            command.Invoke();
        }
    }

    // Debug command with one generic parameter
    public class DebugCommand<T1> : DebugCommandBase
    {
        private readonly Action<T1> command; // Action to be executed with one parameter
    
        // Constructor for the DebugCommand<T1> class
        public DebugCommand(string id, string description, string format, Action<T1> command) 
            : base(id, description, format)
        {
            this.command = command;
        }

        // Method to invoke the command's action with one parameter
        public void Invoke(T1 value)
        {
            command.Invoke(value);
        }
    }
}
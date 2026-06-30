# Mixo Ndukwane
# ST10480403
# Cybersecurity Awareness Chatbot

Project Discription

A GUI-based cybersecurity chatbot developed in C# using WinForms to educate users about online safety through interactive conversations, keyword recognition, sentiment detection, and memory features.

Overview

The CyberSecurity ChatBot is a console-based application designed to educate users about common cybersecurity topics such as phishing, malware, passwords, and safe browsing.
It interacts with users, provides helpful responses, and promotes awareness of online safety.

1.Initial Project Setup
-Created the base project structure
-Configured the .NET environment
-Added main entry point (Program.cs)

2.Added Voice Greeting
-Implemented a feature where the chatbot greets the user with audio
-Improved user engagement and interactivity

3. Added ASCII Art
Designed a visual text-based interface using ASCII art
Made the console UI more appealing

4.Implemented User Input
-Allowed users to type and send messages
-Added logic to capture and process input

5. Added Chatbot Responses
-Implemented response logic based on user queries
-Introduced cybersecurity knowledge into the system

6.Improved UI and Validation
-Enhanced user interface flow
-Added input validation to prevent errors
-Improved overall user experience
# chatbot part2
1. created a winframe project and added keywordResponders, memeorystore, and sentiment Ditector classes
2. completed the GUI layout
3. created a KeywordResponder class and added 10+ keywords
4. created SentimentDetector and MemoryStore classes
5. connected the mainwindow to the chatbot GUI
   
## Features included:
- Interactive GUI chatbot interface
- Cybersecurity keyword recognition
- Responses for topics such as phishing, malware, passwords, VPNs, encryption, and ransomware
- Sentiment detection for user interaction
- Memory storage for personalized conversations
- WAV greeting sound implementation
- Event-driven chatbot communication
- User-friendly design with organized layout
## Prerequisites
Before running the project, ensure you have the following installed:

- Windows Operating System
- Visual Studio 2022
- .NET 8.0 SDK
- Git (optional, for cloning the repository)
## Clone and Run the Project
### Step 1 - Clone the Repository
Open a terminal or Git Bash and run:
git clone https://github.com/Mixo-Ndukwane/your-chatbot.git
### Step 2 - Open the Project
-Open Visual Studio 2022
-Click Open a project or solution
-Navigate to the cloned project folder
-Open the .sln solution file
### Step 3 - Restore Dependencies
-After opening the solution:
-Go to Build
-Select Build Solution
-Wait for NuGet packages and dependencies to restore
### Step 4 - Run the Application
-The chatbot GUI application should now launch.
## WAV Greeting File Setup
-Place the greeting.wav file inside the main project folder.
-The file should be located in the same folder as the .csproj file.
Example:
ChatBot_GUI/
│
├── ChatBot_GUI.csproj
├── greeting.wav
├── Form1.cs
└── Program.cs
-In Visual Studio:
Right-click greeting.wav
Select Properties
Set:
Build Action → Content
Copy to Output Directory → Copy if newer
This ensures the WAV file is available when the application runs.
## screenshot of the running GUI
<img width="1358" height="719" alt="image" src="https://github.com/user-attachments/assets/7f06a39e-7a11-4f39-a95a-1f1fe49f281a" />
## youtube link
https://youtu.be/rsqVcENQ45k?si=4v0CWTsenFyvQZzm

# chatbot part3
## Features
- Task Assistant
- Task reminders
- Save tasks automatically
- Complete tasks
- Delete tasks
- Cybersecurity Quiz
- Multiple-choice questions
- Quiz scoring
- Quiz explanations
- Natural Language Processing (NLP) simulation
- Activity Log
- User interaction history

---

## Technologies Used

- C#
- .NET Framework 4.8
- Windows Forms
- Newtonsoft.Json
- Visual Studio 2022

---

## Prerequisites

Before running the project, install:

- Visual Studio 2022
- .NET Framework 4.8
- Newtonsoft.Json NuGet Package

---

## Installing Newtonsoft.Json

1. Open the project in Visual Studio.

2. Click

```
Tools
```

↓

```
NuGet Package Manager
```

↓

```
Manage NuGet Packages for Solution
```

3. Search for

```
Newtonsoft.Json
```

4. Install the latest stable version.

---

##Did this after setup
 Added JSON task storage with TaskStorageHelper and CyberTask model 

## Running the Project

1. Clone the repository

```bash
git clone https://github.com/Mixo-Ndukwane/chatbot.git
```

2. Open the solution in Visual Studio.

3. Restore NuGet packages if prompted.

4. Press

```
F5
```

or

```
Start
```

to run the chatbot.

---

## Task Storage

No setup is required.

The chatbot automatically creates

```
tasks.json
```

the first time a task is added.

Tasks are automatically loaded each time the application starts.

---

## Project Structure

```
ChatBot_GUI
│
├── Form1.cs
├── BubblePanel.cs
├── MemoryStore.cs
├── KeywordResponder.cs
├── SentimentDetector.cs
├── QuizManager.cs
├── QuizQuestion.cs
├── TaskManager.cs
├── CyberTask.cs
├── TaskStorageHelper.cs
├── ActivityLogger.cs
├── NLPProcessor.cs
├── Intent.cs
├── SoundPlayerService.cs
├── greeting.wav
└── tasks.json (created automatically)
```

---

# How to Use

### Ask Cybersecurity Questions

Examples:

```
What is phishing?
```

```
Explain malware
```

```
Tell me about VPNs
```

---

### Manage Tasks

Examples:

```
add task Enable 2FA
```

```
show tasks
```

```
complete task 1
```

```
delete task 1
```

---

### Quiz

Start the quiz from the GUI.

The quiz includes:

- Multiple choice questions
- True/False questions
- Score tracking
- Feedback after every answer

---

### Activity Log.

Commands:

```
show activity log
```

```
show history
```

```
show more
```

---

## Screenshot of Running Application
<img width="1366" height="717" alt="image" src="https://github.com/user-attachments/assets/ce28a20a-8c48-4f5e-9ecc-63e36836ab5d" />
##Youtube Video
https://youtu.be/XUasNzzx7wo?si=sz99v7Uwlcv9fesq





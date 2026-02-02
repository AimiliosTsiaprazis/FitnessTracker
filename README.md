# FitnessTracker

**FitnessTracker UI** is a C#/.NET project that provides a graphical interface for tracking workouts.  
It includes **user registration, login, exercise selection, filtering, and workout tracking**.  

## Features
- User registration and login  
- Exercise selection and filtering  
- Tracking and viewing user workouts  
- Interactive and user-friendly UI  

## Prerequisites
- **.NET SDK** installed on your system  
  Download it from [Microsoft .NET Downloads](https://dotnet.microsoft.com/download)  

## Setup
1. Download and install any missing frameworks, mostly **.NET**, if prompted.  
2. Add your **API keys and host configuration from RapidApi** in the following files: https://rapidapi.com/justin-WFnsXH_t6/api/exercisedb 
   - `Index.cshtml.cs`  
   - `Profile.cshtml.cs`  
   - `User_Exercises.cshtml.cs`  
   - `View_User_Exercise.cshtml.cs`  
3. Open `Program.cs` in your IDE and run the project **OR** navigate to the project folder in the terminal:  
   ```bash
   cd FitnessTracker
   dotnet run

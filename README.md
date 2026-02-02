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
2. Set up the MySQL database:

-- Create database
DROP DATABASE IF EXISTS fitness_tracker;
CREATE DATABASE fitness_tracker;
USE fitness_tracker;

-- Users Table
CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(100) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL
);

-- User_Exercises Table
CREATE TABLE user_exercises (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    exercise_id VARCHAR(50) NOT NULL,
    name VARCHAR(150) NOT NULL,
    body_part VARCHAR(100),
    equipment VARCHAR(100),
    gif_url TEXT,
    target VARCHAR(100),
    -- JSON-Spalten für Listen aus deinem Model
    secondary_muscles JSON,
    instructions JSON,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);


3. Add your **API keys and host configuration from RapidApi** in the following files: https://rapidapi.com/justin-WFnsXH_t6/api/exercisedb 
   - `Index.cshtml.cs`  
   - `Profile.cshtml.cs`  
   - `User_Exercises.cshtml.cs`  
   - `View_User_Exercise.cshtml.cs`  
4. Open `Program.cs` in your IDE and run the project **OR** navigate to the project folder in the terminal:  
   ```bash
   cd FitnessTracker
   dotnet run

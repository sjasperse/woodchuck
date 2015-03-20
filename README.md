# Woodchuck
Rich logging context, every time.

## What is Woodchuck?
 Woodchuck is a logging helper framework which is built to try to give rich context to each and every log entry. It will depend on log4net because that is was I know, although it looks like swipping in NLog would not be difficult if there is a good reason for me to do that.

## Why is this helpful?
 I want great logging. If there are going to be bugs, either in development or production, I want my logs to have all the information I need to solve it. I wanted a logging framework that allowed me to easily have lots of context about everything to help me isolate and fix bugs quickly, or help other integrating teams know what is happening.

## How will it work?
 The vision for this involves three things:
1. Easily provide additional information about a logging event.
1. By default, include as much additional contextual information useful for a given application.
1. Be able to log in a way that is easy to read.

## So, now what?
Well, this is yet another side project for me, so I can't always prioritize it the way I would like.

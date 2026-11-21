# ASP.NET Kanto Pokedex

[![Build and deploy](https://github.com/frankg2694/AspNet-Kanto-Pokedex/actions/workflows/build-deploy.yml/badge.svg)](https://github.com/frankg2694/AspNet-Kanto-Pokedex/actions/workflows/build-deploy.yml)

This repository contains a web application that showcases a party of six Pokemon from the original 151. It uses React on the frontend and ASP.NET Core on the backend, and it obtains the Pokemon data from PokeAPI. Each Pokemon Card has a Dropdown, allowing users to switch any Pokemon in the party for another one from the 151 species in Kanto. The backend caches the PokeAPI responses for 7 days using HybridCache (memory + Redis). If Redis is not running or functional, the application will get the data from PokeAPI.

Before using this application locally, ensure that you have the .NET 11 SDK and Node.js installed. Once verified, install all dependencies in the client directory (React) through the CLI by running the command `npm install` in `src/pokedex.client`.

The Redis functionality is optional. To enable it, make sure you have installed Docker Desktop in your computer, and create the Redis container through the CLI by running the command `docker run -d --name pokedex-redis -p 6379:6379 --restart unless-stopped redis:8`. If the container is stopped, run `docker start pokedex-redis`.

To use the current build locally, follow these steps:

1. In a CLI window, navigate to the `src/Pokedex.Server` directory and run the command `dotnet run --launch-profile http`
2. In a second CLI window, navigate to the `src/pokedex.client` directory and run the command `npm run dev`
3. In your browser, go to `http://localhost:35630/`

The tests do not need Redis or internet access. To run the tests locally, follow these steps:

1.  In a CLI window, navigate to the home directory and run the command `dotnet test` to run the ASP.NET Core tests
2.  In a second CLI window, navigate to the `src/pokedex.client` directory and run the command `npm test` to run the React tests

You can view this app hosted in Azure [here](https://aspnet-kanto-pokedex-frankg2694-ghe8dcg7d6cngpf9.westus3-01.azurewebsites.net/).
// Send a GET request and return the JSON
async function getJson(url) {
    // Call the Server and wait for the response
    const response = await fetch(url)

    // 503 means PokeAPI is down
    if (response.status === 503) {
        const message = await response.text()
        throw new Error(message)
    }

    // Error check in case the request did not come back OK
    if (!response.ok) {
        throw new Error(`Request failed with status ${response.status}`)
    }

    return response.json()
}

// GET /api/pokemon, get the Dex Number and Name of all 151 Pokemon
export function fetchPokemonList() {
    return getJson('/api/pokemon')
}

// GET /api/pokemon/25, get one Pokemon by its Dex Number
export function fetchPokemon(dexNum) {
    return getJson(`/api/pokemon/${dexNum}`)
}

// GET /api/pokemon/random, get a random Pokemon
export function fetchRandomPokemon() {
    return getJson('/api/pokemon/random')
}

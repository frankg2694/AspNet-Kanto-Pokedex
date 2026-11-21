import { useEffect, useState } from 'react'
import { fetchPokemon, fetchRandomPokemon } from '../api/pokemon'
import PokemonSelect from './PokemonSelect'

export default function PokemonEntry({ pokemonList }) {
    // State of the Card: the Pokemon shown, if it is loading, and the error message if any
    const [pokemon, setPokemon] = useState(null)
    const [loading, setLoading] = useState(true) // Controls the loading animations
    const [error, setError] = useState(null)

    // Wait for the request to finish, then show the Pokemon or the error
    async function showPokemon(request) {
        setLoading(true)
        setError(null)
        try {
            const result = await request
            setPokemon(result)
        } catch (err) {
            setError(err.message)
        } finally {
            setLoading(false) // Stop loading even if there was an error
        }
    }

    // Load a random Pokemon when the Card first appears
    useEffect(() => {
        // Flag set to true by the cleanup
        let ignore = false;

        (async () => {
            try {
                const result = await fetchRandomPokemon()
                if (!ignore) setPokemon(result)
            } catch (err) {
                if (!ignore) setError(err.message)
            } finally {
                if (!ignore) setLoading(false) // Stop loading even if there was an error
            }
        })()

        // Cleanup
        return () => {
            ignore = true
        }

    }, [])

    // Event handles picking a Pokemon in the dropdown
    function handleSelect(dexNum) {
        showPokemon(fetchPokemon(dexNum))
    }

    // Event handles the "Switch Pokemon" button
    function handleSwitch() {
        showPokemon(fetchRandomPokemon())
    }

    return (
        <div className="card pokemon-card shadow-sm">
            <div className="card-body d-flex flex-column">
                {/* Show the error, else the Pokemon, else a spinner on the first load */}
                {error ? (
                    <div className="alert alert-danger" role="alert">
                        Could not load a Pokemon: {error}
                    </div>
                ) : pokemon ? (
                    <>
                        <h2 className="card-title h3">
                            {pokemon.name} <span className="text-body-secondary">#{pokemon.dexNum}</span>
                        </h2>
                        <p className="mb-3">
                            <span className="badge text-bg-secondary me-1">{pokemon.typeOne}</span>
                            {/* Second badge only shows if the Pokemon has a second type */}
                            {pokemon.typeTwo && <span className="badge text-bg-secondary">{pokemon.typeTwo}</span>}
                        </p>
                        <img
                            className="pokemon-image mb-3"
                            src={pokemon.picture}
                            alt={`Official artwork of ${pokemon.name}`}
                        />
                    </>
                ) : (
                    <div className="flex-grow-1 d-flex align-items-center justify-content-center py-5">
                        <div className="spinner-border" role="status">
                            <span className="visually-hidden">Loading...</span>
                        </div>
                    </div>
                )}

                <div className="mt-auto">
                    {/* No Pokemon yet on the first load so selectedDexNum is null */}
                    <PokemonSelect
                        pokemonList={pokemonList}
                        selectedDexNum={pokemon?.dexNum ?? null}
                        onSelect={handleSelect}
                        disabled={loading}
                    />
                    <button type="button" className="btn btn-primary w-100" onClick={handleSwitch} disabled={loading}>
                        {loading ? 'Loading...' : 'Switch Pokemon'}
                    </button>
                </div>
            </div>
        </div>
    )
}
import { describe, it, expect, vi } from 'vitest'
import { fetchPokemon } from './pokemon'

describe('pokemon api', () => {
    // Check the Pokemon comes back when the Server answers OK
    it('returns the Pokemon from the server when the answer is OK', async () => {
        const mew = {
            dexNum: 151,
            name: 'Mew',
            typeOne: 'Psychic',
            typeTwo: null,
            picture: 'https://example.com/151.png',
        }
        const fetch = vi.fn(async () => new Response(JSON.stringify(mew)))
        vi.stubGlobal('fetch', fetch)

        await expect(fetchPokemon(151)).resolves.toEqual(mew)
        expect(fetch).toHaveBeenCalledWith('/api/pokemon/151')
    })

    // Check an error is thrown when the Server does not answer OK
    it('throws an error when the server does not answer with OK', async () => {
        const fetch = vi.fn(async () => new Response('{}', { status: 400 }))
        vi.stubGlobal('fetch', fetch)

        await expect(fetchPokemon(152)).rejects.toThrow('Request failed with status 400')
        expect(fetch).toHaveBeenCalledWith('/api/pokemon/152')
    })

    // Check the Server's message is thrown when PokeAPI is down (503)
    it('throws the server message when PokeAPI is down', async () => {
        const fetch = vi.fn(async () => new Response('PokeAPI is not available right now, please try again later', { status: 503 }))
        vi.stubGlobal('fetch', fetch)

        await expect(fetchPokemon(25)).rejects.toThrow('PokeAPI is not available right now, please try again later')
    })
})
import { afterEach, vi } from 'vitest'
import { cleanup } from '@testing-library/react'
import '@testing-library/jest-dom/vitest'

// Runs after every test, so the next test starts fresh
afterEach(() => {
    // Remove whatever the test rendered
    cleanup()
    // Put back the real fetch in case the test replaced it with a fake one
    vi.unstubAllGlobals()
})
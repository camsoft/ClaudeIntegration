import { useState } from 'react';
import './App.css';
import logo from './assets/logo.png';

const DEFAULT_SYSTEM_PROMPT = 'You are an expert programmer';

type Mode = 'prompt' | 'expert';

function App() {
    const [mode, setMode] = useState<Mode>('prompt');
    const [prompt, setPrompt] = useState('');
    const [systemPrompt, setSystemPrompt] = useState(DEFAULT_SYSTEM_PROMPT);
    const [userQuestion, setUserQuestion] = useState('');
    const [response, setResponse] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    async function submitPrompt() {
        if (isLoading) {
            return;
        }

        if (mode === 'prompt' && !prompt.trim()) {
            return;
        }

        if (mode === 'expert' && (!systemPrompt.trim() || !userQuestion.trim())) {
            return;
        }

        setIsLoading(true);
        setResponse('');

        try {
            const endpoint = mode === 'prompt' ? 'api/claude/prompt' : 'api/claude/expert';
            const body = mode === 'prompt'
                ? JSON.stringify(prompt)
                : JSON.stringify({ systemPrompt, userQuestion });

            const result = await fetch(endpoint, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body
            });

            if (result.ok) {
                const data = await result.json();
                setResponse(data.response);
            } else {
                setResponse(`Error: ${result.status} ${result.statusText}`);
            }
        } catch (error) {
            setResponse(`Error: ${error}`);
        } finally {
            setIsLoading(false);
        }
    }

    return (
        <div>
            <img src={logo} alt="Training logo" className="logo" />
            <h1>Claude Integration</h1>
            <p>Choose a mode below, enter your prompt(s), and submit it to Claude.</p>
            <div className="mode-select-container">
                <label htmlFor="mode-select">Mode: </label>
                <select
                    id="mode-select"
                    value={mode}
                    onChange={e => {
                        setMode(e.target.value as Mode);
                        setResponse('');
                    }}
                >
                    <option value="prompt">Simple Prompt</option>
                    <option value="expert">Expert Prompt (System + User)</option>
                </select>
            </div>
            {mode === 'prompt' && (
                <div>
                    <textarea
                        aria-label="Prompt"
                        value={prompt}
                        onChange={e => setPrompt(e.target.value)}
                        rows={4}
                        style={{ width: '100%' }}
                        placeholder="Ask Claude something..."
                    />
                </div>
            )}
            {mode === 'expert' && (
                <>
                    <div>
                        <textarea
                            aria-label="System Prompt"
                            value={systemPrompt}
                            onChange={e => setSystemPrompt(e.target.value)}
                            rows={2}
                            style={{ width: '100%' }}
                            placeholder="Set Claude's role/expertise..."
                        />
                    </div>
                    <div>
                        <textarea
                            aria-label="User Prompt"
                            value={userQuestion}
                            onChange={e => setUserQuestion(e.target.value)}
                            rows={4}
                            style={{ width: '100%' }}
                            placeholder="Ask Claude something..."
                        />
                    </div>
                </>
            )}
            <div>
                <button className="submit-button" onClick={submitPrompt} disabled={isLoading}>
                    {isLoading ? 'Submitting...' : 'Submit'}
                </button>
            </div>
            <div>
                <textarea
                    aria-label="Response"
                    value={response}
                    readOnly
                    rows={20}
                    style={{ width: '100%' }}
                    placeholder="Claude's response will appear here..."
                />
            </div>
        </div>
    );
}

export default App;
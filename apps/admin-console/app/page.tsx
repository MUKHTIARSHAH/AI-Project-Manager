const apiBase = process.env.NEXT_PUBLIC_AIPM_API_BASE ?? "http://localhost:5000";

async function readEndpoint(path: string): Promise<{ ok: boolean; status: number; body: string }> {
  try {
    const response = await fetch(`${apiBase}${path}`, { cache: "no-store" });
    const body = await response.text();
    return { ok: response.ok, status: response.status, body };
  } catch (error) {
    return { ok: false, status: 0, body: String(error) };
  }
}

export default async function HomePage() {
  const checks = await Promise.all([
    readEndpoint("/health"),
    readEndpoint("/ready"),
    readEndpoint("/api/v1/platform/deployment"),
    readEndpoint("/api/v1/agent-types")
  ]);

  const labels = ["/health", "/ready", "/api/v1/platform/deployment", "/api/v1/agent-types"];

  return (
    <main>
      <h1>AIPM Admin Shell (M6 Minimal)</h1>
      <p>Connectivity check against existing platform APIs.</p>
      <ul>
        {checks.map((check, idx) => (
          <li key={labels[idx]}>
            <strong>{labels[idx]}</strong>: {check.ok ? "OK" : "FAILED"} (status: {check.status})
          </li>
        ))}
      </ul>
      <p style={{ color: "#666" }}>API Base: {apiBase}</p>
    </main>
  );
}

# GitHub repository setup checklist

Manual steps to complete GitHub configuration for [AI-Project-Manager](https://github.com/MUKHTIARSHAH/AI-Project-Manager). Items marked **done** are already in the repository.

---

## Already configured in repo

- [x] CI workflow (`.github/workflows/ci.yml`) — build, test, format, coverage
- [x] CodeQL workflow (`.github/workflows/codeql.yml`)
- [x] Dependabot (`.github/dependabot.yml`)
- [x] Apache 2.0 license (`LICENSE`)
- [x] Professional README with badges
- [x] Release notes draft (`docs/releases/v0.2.1-foundation-freeze.md`)
- [x] Git tag `v0.2.1-foundation-freeze`

---

## 1. Create GitHub Release (manual)

1. Go to **Releases** → **Draft a new release**
2. Choose tag: `v0.2.1-foundation-freeze`
3. Title: `v0.2.1 — Foundation Freeze`
4. Paste body from [docs/releases/v0.2.1-foundation-freeze.md](releases/v0.2.1-foundation-freeze.md)
5. Publish release

Or with GitHub CLI (after `gh auth login`):

```bash
gh release create v0.2.1-foundation-freeze \
  --title "v0.2.1 — Foundation Freeze" \
  --notes-file docs/releases/v0.2.1-foundation-freeze.md
```

---

## 2. Enable repository features

**Settings → General → Features:**

- [x] Issues — enable
- [ ] Discussions — optional
- [ ] Projects — enable if using GitHub Projects for roadmap
- [ ] Wiki — optional (docs live in `docs/`)

---

## 3. Add repository topics

**Settings → General → Topics**, add:

```
aspnet-core
dotnet
clean-architecture
ddd
cqrs
mediatr
mass-transit
entity-framework-core
ai
project-management
multi-tenant
workflow
csharp
```

CLI:

```bash
gh repo edit MUKHTIARSHAH/AI-Project-Manager \
  --add-topic aspnet-core,dotnet,clean-architecture,ddd,cqrs,mediatr,mass-transit,entity-framework-core,ai,project-management,multi-tenant,workflow,csharp
```

---

## 4. Branch protection on `main`

**Settings → Branches → Add rule** for `main`:

- [x] Require a pull request before merging
- [x] Require status checks to pass: **CI / build-and-test**
- [x] Do not allow bypassing the above settings
- [x] Block force pushes

CLI (requires admin token):

```bash
gh api repos/MUKHTIARSHAH/AI-Project-Manager/branches/main/protection \
  --method PUT \
  --field required_status_checks='{"strict":true,"contexts":["build-and-test"]}' \
  --field enforce_admins=true \
  --field required_pull_request_reviews='{"required_approving_review_count":0}' \
  --field restrictions=null \
  --field allow_force_pushes=false
```

---

## 5. Pin repository on profile

1. Go to your GitHub profile
2. **Customize your pins**
3. Pin **AI-Project-Manager**

---

## 6. Repository description

Suggested description:

> Enterprise AI project management control plane — Clean Architecture, DDD, CQRS, multi-tenant .NET 9 platform with automated tests and architecture validation.

```bash
gh repo edit MUKHTIARSHAH/AI-Project-Manager \
  --description "Enterprise AI project management control plane — Clean Architecture, DDD, CQRS, multi-tenant .NET 9 platform."
```

---

## 7. Verify clean clone

Confirmed on 2026-07-08:

```bash
git clone --depth 1 --branch v0.2.1-foundation-freeze \
  https://github.com/MUKHTIARSHAH/AI-Project-Manager.git

cd AI-Project-Manager
dotnet restore src/AIPM.sln
dotnet build src/AIPM.sln -c Release
dotnet test src/AIPM.sln -c Release
```

**Result:** Build succeeded, **76/76 tests passed**.

---

## License note

This repository uses **Apache License 2.0** (not MIT). Both are standard open-source licenses. Changing to MIT would require updating `LICENSE` and all license references — only do so if you explicitly want MIT terms.

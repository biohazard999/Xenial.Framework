---
title: Installation
---

<code-group>
<code-block title=".NET CLI">

<div class="language-bash"><pre class="language-bash"><code>dotnet add package Xenial.Framework --version {{ $var['xenialVersion'] }}</code></pre></div>

</code-block>


<code-block title="PackageReference">

<div class="language-xml"><pre class="language-xml"><code>&ltPackageReference Include="Xenial.Framework" Version="{{ $var['xenialVersion'] }}" /&gt</code></pre></div>

</code-block>

<code-block title="Package Manager">

<div class="language-powershell"><pre class="language-powershell"><code>Install-Package Xenial.Framework -Version {{ $var['xenialVersion'] }}</code></pre></div>

</code-block>

<code-block title="Paket CLI">

<div class="language-bash"><pre><code>paket add Xenial.Framework --version {{ $var['xenialVersion'] }}</code></pre></div>

</code-block>

</code-group>

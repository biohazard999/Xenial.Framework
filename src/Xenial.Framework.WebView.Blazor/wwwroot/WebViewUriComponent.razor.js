class XFrameBypass extends HTMLElement {

  /// <summary> Observed attributes. </summary>
  ///
  /// <returns> A get. </returns>

  static get observedAttributes() {
    return ['src', 'style']
  }
  /// <summary> Default constructor. </summary>
  constructor() {

    /// <summary>   Supers this object. </summary>
    ///
    /// <returns>   . </returns>

    super()
    let style = this.attributes['style'];
    if (style && style.textContent) {
      style = style.textContent;

    /// <summary>   Gets the else. </summary>
    ///
    /// <value> . </value>

    } else {
      style = '';
    }

    /// <summary>   Attach shadow. </summary>
    ///
    /// <param name="{ mode">   The mode. </param>
    ///
    /// <returns>   . </returns>

    this.attachShadow({ mode: 'open' }).innerHTML = `<iframe style='${style}'></iframe>`
  }

  /// <summary> Callback, called when the connected. </summary>
  ///
  /// <returns> . </returns>

  connectedCallback() {
    setTimeout(() => {
      const iframe = this.shadowRoot.querySelector('iframe');
      if (iframe && iframe.contentDocument) {
        iframe.contentDocument.body.innerHTML = this.innerHTML
      } else {
        let style = this.attributes['style'];
        if (style && style.textContent) {
          style = style.textContent;
        } else {
          style = '';
        }
        this.innerHTML = `<iframe style='${style}'></iframe>`;
        this.load(this.attributes['src']);
      }
    });
  }

  /// <summary> Callback, called when the attribute changed. </summary>
  ///
  /// <param name="name">       The name. </param>
  /// <param name="oldValue">   The old value. </param>
  /// <param name="newValue">   The new value. </param>
  ///
  /// <returns> . </returns>

  attributeChangedCallback(name, oldValue, newValue) {
    if (name === 'src') {
      this.shadowRoot.querySelector('iframe').remove();

      let style = this.shadowRoot.host.attributes['style'];
      if (style && style.textContent) {
        style = style.textContent;
      } else {
        style = '';
      }

      this.shadowRoot.innerHTML = `<iframe style='${style}'></iframe>`;
      this.load(newValue)
    }
    if (name === 'style') {
      this.shadowRoot.innerHTML = `<iframe style='${newValue}'></iframe>`;
      this.load(this.attributes['src'])
    }

  }

  /// <summary> Loads the given document. </summary>
  ///
  /// <param name="url">        URL of the resource. </param>
  /// <param name="options">    Options for controlling the operation. </param>
  ///
  /// <returns> . </returns>

  load(url, options) {
    if (url && url.textContent) {
      url = url.textContent;
    }
    if (url && !url.startsWith('http')) {
      throw new Error(`X-Frame-Bypass src ${url} does not start with http(s)://`)
    }
    if (!url) {
      return;
    }
    console.log('X-Frame-Bypass loading:', url)
    const contentDocForLoading = this.shadowRoot.querySelector('iframe').contentDocument;
    if (!contentDocForLoading || !contentDocForLoading.body) {
      console.log("cannot show loading animation, body is null");
    } else {
      contentDocForLoading.body.innerHTML = `<html>
<head>
	<style>
	.loader {
		position: absolute;
		top: calc(50% - 25px);
		left: calc(50% - 25px);
		width: 50px;
		height: 50px;
		background-color: #333;
		border-radius: 50%;  
		animation: loader 1s infinite ease-in-out;
	}
	@keyframes loader {
		0% {
		transform: scale(0);
		}
		100% {
		transform: scale(1);
		opacity: 0;
		}
	}
	</style>
</head>
<body>
	<div class="loader"></div>
</body>
</html>`;
    }

    this.fetchProxy(url, options, 0).then(res => res.text()).then(data => {
      if (data) {
        const basePatch = `<base href="${url}">`;
        let basePatched = data.replace(/<base([^>]*)>/i, basePatch);

        if (!/<base([^>]*)>/i.test(basePatched)) {
          basePatched = basePatched.replace(/<head>/i, `$&${basePatch}`);
        }

        const n = basePatched.lastIndexOf("</head>");

        const headerPatch = `<script>
	// X-Frame-Bypass navigation event handlers
	document.addEventListener('click', e => {
		if (frameElement && document.activeElement && document.activeElement.href) {
			e.preventDefault()
			frameElement.load(document.activeElement.href)
		}
	})
	document.addEventListener('submit', e => {
		if (frameElement && document.activeElement && document.activeElement.form && document.activeElement.form.action) {
			e.preventDefault()
			if (document.activeElement.form.method === 'post')
				frameElement.load(document.activeElement.form.action, {method: 'post', body: new FormData(document.activeElement.form)})
			else
				frameElement.load(document.activeElement.form.action + '?' + new URLSearchParams(new FormData(document.activeElement.form)))
		}
	})
	</script>`;

        var patchedHtml = basePatched.substring(0, n) + headerPatch + basePatched.substring(n);
        const iframe = this.shadowRoot.querySelector('iframe');
        const contentDocument = iframe.contentDocument;
        if (!contentDocument || !contentDocument.body) {
          console.error("contentDocument.body is null");
          iframe.innerHTML = patchedHtml
        } else {
          iframe.contentDocument.body.innerHTML = patchedHtml;
        }
      }
    }).catch(e => console.error('Cannot load X-Frame-Bypass:', e))
  }

  /// <summary> Fetches a proxy. </summary>
  ///
  /// <param name="url">        URL of the resource. </param>
  /// <param name="options">    Options for controlling the operation. </param>
  /// <param name="i">          Zero-based index of the. </param>
  ///
  /// <returns> The proxy. </returns>

  fetchProxy(url, options, i) {
    const proxies = (options || {}).proxies || [
      'https://cors-anywhere.herokuapp.com/',
      'https://yacdn.org/proxy/',
      'https://api.codetabs.com/v1/proxy/?quest='
    ]
    return fetch(proxies[i] + url, options).then(res => {
      if (!res.ok)
        throw new Error(`${res.status} ${res.statusText}`);
      return res
    }).catch(error => {
      if (i === proxies.length - 1)
        throw error
      return this.fetchProxy(url, options, i + 1)
    })
  }
}

/// <summary>   Loads web view URI component. </summary>
///
/// <returns>   The web view URI component. </returns>

export function loadWebViewUriComponent() {
  console.log("loading WebViewUriComponent");
  const xFrameBypass = customElements.get('x-frame-bypass');
  if (!xFrameBypass) {
    customElements.define('x-frame-bypass', XFrameBypass);
  }
}

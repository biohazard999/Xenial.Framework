module.exports = {
    title: "Xenial.Framework",
    description: "Easy. Flexible. Focused.",
    serviceWorker: true,
    themeConfig: {
        logo: '/images/logo.svg',
        lastUpdated: 'Last Updated',
        repo: 'xenial-io/Xenial.Framework',
        docsDir: 'docs',
        docsBranch: 'master',
        editLinks: true,
        editLinkText: 'Help us improve this page!',
        searchPlaceholder: 'Search...',
        smoothScroll: true,
        nav: [
            { text: 'Home', link: '/' },
            { text: 'Guide', link: '/guide/' },
            { text: 'Blog', link: 'https://blog.xenial.io/tags/Xenial/' },
            { text: 'Support', link: 'https://github.com/xenial-io/Xenial.Framework/issues/' },
            { text: 'Buy', link: 'https://stage.xenial.io/pricing/' },
        ],
        sidebar: [
            '/',
            '/guide/',
            '/installation/'
        ]
    },
    markdown: {
        lineNumbers: true,
        toc: { includeLevel: [1, 2] },
    },
    plugins: [
        ['vuepress-plugin-global-variables', { variables: { xenialVersion: '1.0.0' } }],
        ['vuepress-plugin-code-copy', true]
    ],
}
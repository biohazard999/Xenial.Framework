const git = require('git-rev-sync');
let gitTag = git.tag();

if (gitTag) {
    gitTag = gitTag.substring(1);
}

module.exports = {
    title: "Xenial.Framework",
    description: "Easy. Flexible. Focused.",
    serviceWorker: true,
    host: "127.0.0.1",
    themeConfig: {
        logo: '/images/logo.svg',
        lastUpdated: 'Last Updated',
        repo: 'xenial-io/Xenial.Framework',
        docsDir: 'docs',
        docsBranch: 'main',
        editLinks: true,
        editLinkText: 'Help us improve this page!',
        searchPlaceholder: 'Search...',
        smoothScroll: true,
        nav: [
            { text: 'Home', link: '/' },
            { text: 'Guide', link: '/guide/' },
            { text: 'Blog', link: 'https://blog.xenial.io/tags/Xenial/' },
            { text: 'Support', link: 'https://github.com/xenial-io/Xenial.Framework/issues/' },
            { text: 'Demo', link: 'https://framework.featurecenter.xenial.io/' },
            { text: 'Buy', link: 'https://stage.xenial.io/pricing/' },
        ],
        sidebar: {
            '/guide/': [
                {
                    title: 'Overview',
                    collapsable: false,
                    children: [
                        '',
                        'module-structure',
                    ]
                },
                {
                    title: 'ModelBuilders',
                    collapsable: false,
                    children: [
                        ['model-builders', 'Introduction'],
                        ['model-builders-inline', 'Inline approach'],
                        ['model-builders-buddy', 'Buddy class approach'],
                        ['model-builders-managers', 'Managers'],
                    ]
                },
                {
                    title: 'Installation',
                    collapsable: false,
                    children: [
                        'installation'
                    ]
                },
                {
                    title: 'Modules',
                    collapsable: false,
                    children: [
                        'installation'
                    ]
                },
                {
                    title: 'Lab',
                    collapsable: false,
                    children: [
                        'installation'
                    ]
                }
            ]
        }
    },
    markdown: {
        lineNumbers: true
    },
    plugins: [
        ['vuepress-plugin-global-variables', { variables: { xenialVersion: gitTag } }],
        ['vuepress-plugin-code-copy', true],
        ['@vuepress/back-to-top'],
        ['@vuepress/nprogress']
    ],
}
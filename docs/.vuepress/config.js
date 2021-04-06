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
        algolia: {
            apiKey: '82712b6e9d51edab5ef22e17d2eaac0c',
            indexName: 'xenial'
        },
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
                        ['model-builders-managers', 'BuilderManagers'],
                        ['model-builders-conventions', 'Conventions'],
                        ['model-builders-advanced', 'Advanced'],
                        ['model-builders-built-in', 'Built-in Attributes'],
                    ]
                },
                {
                    title: 'DetailViewLayoutBuilders',
                    collapsable: false,
                    children: [
                        ['layout-builders', 'Introduction'],
                        ['layout-builders-simple-registration', 'Simple Layout'],
                        ['layout-builders-advanced-syntax', 'LayoutBuilder<T> Syntax'],
                        ['layout-builders-record-syntax', 'Record Syntax'],
                        ['layout-builders-model-builder-syntax', 'ModelBuilder Syntax'],
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
        ['@vuepress/back-to-top'],
        ['@vuepress/nprogress'],
        ['@vuepress/medium-zoom']
    ],
}
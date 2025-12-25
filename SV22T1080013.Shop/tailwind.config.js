/** @type {import('tailwindcss').Config} */
module.exports = {
    darkMode: 'class',
    content: [
        './Views/**/*.cshtml',
        './Pages/**/*.cshtml',
        './wwwroot/**/*.js'
    ],
    theme: {
        extend: {
            colors: {
                primary: '#00d4ff',
                secondary: '#a855f7',
                background: {
                    light: '#f5f8f8',
                    dark: '#0f2023',
                },
                surface: {
                    dark: '#162a2d',
                    light: '#1c3236',
                },
            },
            fontFamily: {
                display: ['Inter', 'sans-serif'],
                body: ['Inter', 'sans-serif'],
            },
        },
    },
    plugins: [],
}

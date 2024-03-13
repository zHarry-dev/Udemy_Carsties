/** @type {import('next').NextConfig} */
const nextConfig = {
    experimental: {
        serverActions: true
    },
    images: {
        domains: [
            "cdn.pixabay.com"
        ],
        remotePatterns: [
            {
                protocol: "http",
                hostname: "**"
            }
        ]
    },
    output: 'standalone'
};

export default nextConfig;

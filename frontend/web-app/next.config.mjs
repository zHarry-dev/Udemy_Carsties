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
    }
};

export default nextConfig;

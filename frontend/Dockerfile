# syntax=docker/dockerfile:1

# Imagen de desarrollo: corre el dev server de Vite con HMR.
# El proyecto es un dashboard interno (SPA); el build de producción optimizado
# (nginx + assets estáticos) queda para cuando se defina el deploy real
# (Fly.io/Railway), ver CLAUDE.md raíz.
FROM node:22-alpine

WORKDIR /app

# Confiar en certificados raíz locales (ej. antivirus con inspección SSL/TLS),
# necesario solo si la red/host intercepta HTTPS hacia registry.npmjs.org. La
# carpeta certs/ está vacía por defecto (gitignored salvo .gitkeep); si no
# hace falta ningún certificado custom, estos pasos no tienen efecto.
COPY certs/ /usr/local/share/ca-certificates/
RUN for f in /usr/local/share/ca-certificates/*.crt; do [ -f "$f" ] && cat "$f" >> /etc/ssl/certs/ca-certificates.crt; done; true
ENV NODE_EXTRA_CA_CERTS=/etc/ssl/certs/ca-certificates.crt

COPY package.json package-lock.json ./
RUN npm ci

COPY . .

EXPOSE 5173

CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]

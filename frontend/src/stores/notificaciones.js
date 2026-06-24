import { defineStore } from 'pinia'

export const useNotificacionesStore = defineStore('notificaciones', {
  state: () => ({
    connection: null,
    items: [],
    noLeidas: 0
  }),
  actions: {
    async conectar(token) {
      if (this.connection) return
      try {
        const { HubConnectionBuilder, LogLevel } = await import('@microsoft/signalr')
        const conn = new HubConnectionBuilder()
          .withUrl('/hubs/notificaciones', { accessTokenFactory: () => token })
          .withAutomaticReconnect()
          .configureLogging(LogLevel.Warning)
          .build()

        conn.on('Notificacion', (notif) => {
          this.items.unshift(notif)
          this.noLeidas++
        })

        await conn.start()
        this.connection = conn
      } catch (e) {
        console.warn('SignalR: no se pudo conectar', e)
      }
    },
    async desconectar() {
      if (this.connection) {
        await this.connection.stop()
        this.connection = null
      }
      this.items = []
      this.noLeidas = 0
    },
    marcarLeidas() {
      this.noLeidas = 0
    }
  }
})

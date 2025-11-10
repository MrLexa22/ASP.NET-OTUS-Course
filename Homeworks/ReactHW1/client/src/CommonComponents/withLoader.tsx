/* eslint-disable @typescript-eslint/no-explicit-any */

import React from 'react'
import type { ComponentType } from 'react'

// HOC: withLoader
// Оборачивает компонент и показывает простой элемент загрузки,
// пока проп `isLoading` истинный. Типизировано для TSX/React.

export function withLoader<P>(WrappedComponent: ComponentType<P>) {
   const ComponentWithLoader: React.FC<P & { isLoading?: boolean; loadingElement?: React.ReactNode }> = (incomingProps) => {
      const { isLoading, loadingElement, ...rest } = incomingProps as any

      if (isLoading) {
         // по умолчанию простой fallback; можно передать свой через loadingElement
         return <>{loadingElement ?? <div>Loading...</div>}</>
      }

      // безопасно рендерим обёрнутый компонент — приводим к any чтобы избежать строгих конфликтов типов
      const WC = WrappedComponent as any
      return <WC {...(rest as P)} />
   }

   ComponentWithLoader.displayName = `withLoader(${WrappedComponent.displayName || WrappedComponent.name || 'Component'})`

   return ComponentWithLoader
}

export default withLoader

import { CanDeactivateFn } from '@angular/router'

export interface DeactivatableComponent {
  canDeactivate: () => boolean | Promise<boolean>
}

export const canDeactivateForm: CanDeactivateFn<DeactivatableComponent> = async (component: DeactivatableComponent) => {
  const canDeactivate = await Promise.resolve(component.canDeactivate());
  if (canDeactivate === false) {
    return confirm('Are you sure to discard changes?');
  }

  return true;
}
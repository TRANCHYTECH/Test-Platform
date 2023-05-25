import { render } from '@testing-library/react';

import VerticalLayout from './vertical-layout';

describe('VerticalLayout', () => {
  it('should render successfully', () => {
    const { baseElement } = render(<VerticalLayout />);
    expect(baseElement).toBeTruthy();
  });
});
